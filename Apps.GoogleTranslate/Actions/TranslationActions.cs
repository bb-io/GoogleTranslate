using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Apps.GoogleTranslate.Utils;
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
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.Actions;

[ActionList("Translation")]
public class TranslationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [BlueprintActionDefinition(BlueprintAction.TranslateText)]
    [Action("Translate text", Description = "Translate a single simple text string using glossary, custom model or adaptive dataset")]
    public async Task<TextTranslationResponse> TranslateText(
        [ActionParameter] BaseTranslationConfig config,
        [ActionParameter] TextTranslationRequest input)
    {
        input.MimeType ??= "text/html";
        config.IgnoreGlossaryCase ??= true;

        if (!string.IsNullOrEmpty(config.CustomModelName))
        {
            if(string.IsNullOrEmpty(config.SourceLanguage))
            {
                throw new PluginMisconfigurationException(
                    "The source language must be specified when using a custom model.");
            }
        }

        var translations = await TranslationBackendFactory.TranslateTextAsync([input.Text], input.MimeType, input.TargetLanguage, config, Client);
        var translation = translations.FirstOrDefault();

        return new TextTranslationResponse
        {
            TranslatedText = translation?.TranslatedText ?? string.Empty,
            DetectedSourceLanguage = translation?.DetectedSourceLanguage ?? config.SourceLanguage ?? string.Empty
        };
    }

    [BlueprintActionDefinition(BlueprintAction.TranslateFile)]
    [Action("Translate", Description = "Translate content retrieved from a CMS or file storage. The output can be used in compatible actions.")]
    public async Task<ContentTranslationResponse> TranslateContent(
        [ActionParameter] BaseTranslationConfig config,
        [ActionParameter] ContentTranslationRequest input)
    {
        input.FileTranslationStrategy ??= "blackbird";
        input.OutputFileHandling ??= "xliff";

        if (input.FileTranslationStrategy == "blackbird")
        {
            try
            {
                return await TranslateInteroperableFile(config, input);
            }
            catch (NotImplementedException)
            {
                return await TranslateFileNatively(config, input);
            }
        }
        else
        {
            return await TranslateFileNatively(config, input);
        }
    }

    private async Task<ContentTranslationResponse> TranslateFileNatively(
        BaseTranslationConfig config,
        ContentTranslationRequest input)
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
            input.File, input.TargetLanguage, config, Client, fileManagementClient);
    }

    private async Task<ContentTranslationResponse> TranslateInteroperableFile(
        BaseTranslationConfig config, 
        ContentTranslationRequest input)
    {
        var stream = await fileManagementClient.DownloadAsync(input.File);
        var contentResult = Transformation.Load(stream, input.File.Name, input.File.ContentType);
        if (!contentResult.Success)
            throw new PluginMisconfigurationException($"The input file could not be processed: {contentResult.Error}");

        var content = contentResult.Value;

        var detectedSourceLanguages = new List<string>();
        var actionResponse = new ContentTranslationResponse();

        async Task<IEnumerable<TranslationDto>> BatchTranslate(IEnumerable<(Unit Unit, Segment Segment)> batch)
        {
            var translations = await TranslationBackendFactory.TranslateTextAsync(
                batch.Select(s => s.Segment.GetSource()), "text/html", input.TargetLanguage, config, Client);

            if (batch.Count() != translations.Count())
                throw new PluginApplicationException("Google translate did not return expected number of translations.");

            return translations;
        }

        var translations = await content
            .GetUnits()
            .Batch(25, x => !x.IsIgnorbale && x.IsInitial)
            .Process(BatchTranslate);

        foreach (var (unit, results) in translations)
        {
            foreach (var (segment, result) in results)
            {
                segment.State = SegmentState.Translated;

                if (!string.IsNullOrEmpty(result.DetectedSourceLanguage))
                    detectedSourceLanguages.Add(result.DetectedSourceLanguage.ToLower());

                segment.SetTarget(result.TranslatedText);
            }
            unit.Provenance.Translation.Tool = "Google Translate";
            unit.Provenance.Translation.ToolReference = "https://translate.google.com/";
        }

        var mostOccuringSourceLanguage = detectedSourceLanguages.Count > 0
                    ? detectedSourceLanguages
                        .GroupBy(s => s)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key
                    : null;

        content.SourceLanguage ??= mostOccuringSourceLanguage ?? string.Empty;
        content.TargetLanguage ??= input.TargetLanguage;

        actionResponse.DetectedSourceLanguage = content.SourceLanguage;

        switch (input.OutputFileHandling)
        {
            case "xliff":
                actionResponse.File = await fileManagementClient.UploadAsync(
                    content.ToStream(),
                    MediaTypes.Xliff2,
                    content.BilingualFileName);
                break;

            case "original":
                var targetContentResult = content.Target();
                if (!targetContentResult.Success)
                    throw new PluginMisconfigurationException($"The translated file could not be restored to its original format: {targetContentResult.Error}");

                var targetContent = targetContentResult.Value;
                actionResponse.File = await fileManagementClient.UploadAsync(
                    targetContent.ToStream(),
                    targetContent.OriginalMediaType ?? input.File.ContentType,
                    targetContent.OriginalName ?? input.File.Name);
                break;

            default:
                throw new PluginMisconfigurationException($"The provided output file handling '{input.OutputFileHandling}' is not supported.");
        }

        return actionResponse;
    }
}
