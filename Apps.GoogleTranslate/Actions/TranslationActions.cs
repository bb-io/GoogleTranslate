using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Apps.GoogleTranslate.Utils.TranslationBackends;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Coders;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;
using HtmlAgilityPack;

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

        return input.FileTranslationStrategy switch
        {
            "blackbird" => await TranslateInteroperableFile(config, input),
            "native" => await TranslateFileNatively(config, input),
            _ => throw new PluginMisconfigurationException($"The provided file translation strategy '{input.FileTranslationStrategy}' is not supported."),
        };
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
        var content = await Transformation.Parse(stream, input.File.Name);

        var detectedSourceLanguages = new List<string>();
        var actionResponse = new ContentTranslationResponse();

        var translatableSegments = content
            .GetUnits()
            .SelectMany(u => u.Segments)
            .Where(x => x.Source.Count > 0 && !x.IsIgnorbale && x.IsInitial);

        foreach (var batch in translatableSegments.Chunk(25))
        {
            var translations = await TranslationBackendFactory.TranslateTextAsync(
                batch.Select(s => s.GetSource()), "text/html", input.TargetLanguage, config, Client);

            if (batch.Count() != translations.Count())
                throw new PluginApplicationException("Google translate did not return expected number of translations.");

            foreach (var (segment, translation) in batch.Zip(translations, (s, t) => (s, t)))
            {
                segment.State = SegmentState.Translated;

                if (!string.IsNullOrEmpty(translation.DetectedSourceLanguage))
                    detectedSourceLanguages.Add(translation.DetectedSourceLanguage.ToLower());

                if (input.PreserveXliffFormatting is not true)
                    segment.SetTarget(translation.TranslatedText);
                else
                    SetSegmentTargetPreservingXliffFormatting(segment, translation.TranslatedText);
            }
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
                    content.Serialize().ToStream(),
                    MediaTypes.Xliff,
                    content.XliffFileName);
                break;

            case "original":
                var targetContent = content.Target();
                actionResponse.File = await fileManagementClient.UploadAsync(
                    targetContent.Serialize().ToStream(),
                    targetContent.OriginalMediaType,
                    targetContent.OriginalName);
                break;

            default:
                throw new PluginMisconfigurationException($"The provided output file handling '{input.OutputFileHandling}' is not supported.");
        }

        return actionResponse;
    }

    #region Temporary XLIFF formatting preservation

    private static void SetSegmentTargetPreservingXliffFormatting(Segment segment, string translatedText)
    {
        // Temportary workaround for preserving inline tags the way OKAPI does it
        // the correct way would be to just segment.SetTarget(translation.TranslatedText); with proper ContentCoder
        segment.ContentCoder = new HtmlContentCoder();
        var sourceTags = segment.Source.Where(c => c is InlineTag).ToList();

        var doc = new HtmlDocument();
        doc.LoadHtml(translatedText);
        var root = doc.DocumentNode;

        if (string.IsNullOrEmpty(root.InnerText))
            return;

        if (root.ChildNodes.Count == 0)
        {
            segment.SetTarget(translatedText);
            return;
        }

        // we have inline tags at this point
        // current encoders doesn't support OKAPI's tags, so we will created TextParts ourselves
        segment.Target.Clear();

        // IMPORTANT:
        // create a single mutable local copy of sourceTags and reuse it for all top-level child nodes.
        // so that same tag will be converted into OKAPI tags with multiple id's
        var localTags = new List<LineElement>(sourceTags);

        foreach (var node in root.ChildNodes)
        {
            foreach (var part in FlattenNodeToParts(node, localTags))
                segment.Target.Add(part);
        }
    }

    private static IEnumerable<LineElement> FlattenNodeToParts(HtmlNode node, IList<LineElement> sourceTags)
    {
        if (node.NodeType == HtmlNodeType.Text)
        {
            yield return new LineElement { Value = node.InnerText };
            yield break;
        }

        if (node.NodeType != HtmlNodeType.Element)
            yield break;

        // Build the opening tag representation the same way original code did
        var openingTagValue = node.EndNode switch
        {
            null => node.OuterHtml,
            _ => node.OuterHtml.Substring(0, node.OuterHtml.IndexOf('>')),
        };

        // Prefer the matching source inline tag for the opening tag (if any)
        var openingTag = sourceTags.FirstOrDefault(t =>
            t.Value.StartsWith(openingTagValue, StringComparison.OrdinalIgnoreCase) &&
            !t.Value.TrimStart().StartsWith("</", StringComparison.Ordinal));

        if (openingTag is not null)
        {
            // remove the matched opening tag from the local copy so it won't be reused for nested/other nodes
            sourceTags.Remove(openingTag);
            yield return openingTag;
        }
        else
        {
            yield return new LineElement { Value = openingTagValue };
        }

        // Flatten children
        foreach (var child in node.ChildNodes)
        {
            foreach (var part in FlattenNodeToParts(child, sourceTags))
                yield return part;
        }

        // Prefer the matching source inline closing tag for this element (if any)
        var closingTag = sourceTags.FirstOrDefault(t =>
            t.Value.StartsWith($"</{node.Name}", StringComparison.OrdinalIgnoreCase));

        if (closingTag is not null)
        {
            // remove the matched closing tag from the local copy so it won't be reused for sibling nodes
            sourceTags.Remove(closingTag);
            yield return closingTag;
        }
        else if (node.EndNode is not null)
        {
            // fallback raw closing tag
            yield return new LineElement { Value = $"</{node.Name}>" };
        }
    }

    #endregion
}