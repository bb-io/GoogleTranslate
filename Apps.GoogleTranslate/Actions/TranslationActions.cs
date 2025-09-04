using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Apps.GoogleTranslate.Utils.TranslationBackends;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;

namespace Apps.GoogleTranslate.Actions;

[ActionList("Translation")]
public class TranslationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [BlueprintActionDefinition(BlueprintAction.TranslateText)]
    [Action("Translate text", Description = "Translate a single simple text string using glossary, custom model or adaptive dataset")]
    public async Task<TextTranslationResponse> TranslateText([ActionParameter] TextTranslationRequest input)
    {
        input.MimeType ??= "text/html";
        input.IgnoreGlossaryCase ??= true;

        var translations = await TranslationBackendFactory.TranslateTextAsync([input.Text], input, Client);
        var translation = translations.FirstOrDefault();

        return new TextTranslationResponse
        {
            TranslatedText = translation?.TranslatedText ?? string.Empty,
            DetectedSourceLanguage = translation?.DetectedSourceLanguage ?? string.Empty
        };
    }

    [BlueprintActionDefinition(BlueprintAction.TranslateFile)]
    [Action("Translate", Description = "Translate content retrieved from a CMS or file storage. The output can be used in compatible actions.")]
    public async Task<ContentTranslationResponse> TranslateContent([ActionParameter] ContentTranslationRequest input)
    {
        input.FileTranslationStrategy ??= "blackbird";
        input.OutputFileHandling ??= "xliff";
        // todo enforce "text/html" during translations

        return input.FileTranslationStrategy switch
        {
            "blackbird" => await TranslateInteroperableFile(input),
            "native" => await TranslateFileNatively(input),
            _ => throw new PluginMisconfigurationException($"The provided file translation strategy '{input.FileTranslationStrategy}' is not supported."),
        };
    }

    private async Task<ContentTranslationResponse> TranslateFileNatively(ContentTranslationRequest input)
    {
        List<string> supportedMimeTypes =
        [
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-powerpoint",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        ];
        
        if (!supportedMimeTypes.Contains(input.File.ContentType))
        {
            throw new PluginMisconfigurationException("The document type is not supported by Google Translate native.");
        }

        return await TranslationBackendFactory.TranslateFileAsync(
            input.File, input, Client, fileManagementClient);
    }

    private async Task<ContentTranslationResponse> TranslateInteroperableFile(ContentTranslationRequest input)
    {
        var stream = await fileManagementClient.DownloadAsync(input.File);
        var content = await Transformation.Parse(stream, input.File.Name);

        var detectedSourceLanguages = new List<string>();
        var actionResponse = new ContentTranslationResponse();

        var translatableSegments = content
            .GetSegments()
            .Where(x => x.GetSource().Length > 0 && !x.IsIgnorbale && x.IsInitial);

        foreach (var batch in translatableSegments.Batch(25))
        {
            var translations = await TranslationBackendFactory.TranslateTextAsync(
                batch.Select(s => s.GetSource()), input, Client);

            if (batch.Count() != translations.Count())
                throw new PluginApplicationException("Google translate did not return expected number of translations.");

            foreach (var (segment, translation) in batch.Zip(translations, (s, t) => (s, t)))
            {
                segment.SetTarget(translation.TranslatedText);
                segment.State = SegmentState.Translated;

                if (!string.IsNullOrEmpty(translation.DetectedSourceLanguage))
                    detectedSourceLanguages.Add(translation.DetectedSourceLanguage.ToLower());
            }
        }

        switch (input.OutputFileHandling)
        {
            case "xliff":
                var mostOccuringSourceLanguage = detectedSourceLanguages.Count > 0
                    ? detectedSourceLanguages
                        .GroupBy(s => s)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key
                    : null;

                content.SourceLanguage ??= mostOccuringSourceLanguage;
                content.TargetLanguage ??= input.TargetLanguage;

                actionResponse.File = await fileManagementClient.UploadAsync(content.Serialize().ToStream(), MediaTypes.Xliff, content.XliffFileName);
                actionResponse.DetectedSourceLanguage = mostOccuringSourceLanguage ?? string.Empty;
                break;

            case "original":
                var targetContent = content.Target();
                actionResponse.File = await fileManagementClient.UploadAsync(targetContent.Serialize().ToStream(), targetContent.OriginalMediaType, targetContent.OriginalName);
                actionResponse.DetectedSourceLanguage = string.Join(",", detectedSourceLanguages.Distinct());
                break;

            default:
                throw new PluginMisconfigurationException($"The provided output file handling '{input.OutputFileHandling}' is not supported.");
        }

        return actionResponse;
    }
}