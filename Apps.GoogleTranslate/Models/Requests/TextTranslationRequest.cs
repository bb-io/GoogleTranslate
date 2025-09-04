using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.GoogleTranslate.Models.Requests;

public class TextTranslationRequest : BaseGoogleTranslationRequest, ITranslateTextInput
{
    [Display("Text")]
    public string Text { get; set; }

    [Display("MIME type", Description = "Type of the text ('text/plain' and 'text/html' supported, default is 'text/html'")]
    public string? MimeType { get; set; }

    public TextTranslationRequest(BaseGoogleTranslationRequest input)
    {
        TargetLanguage = input.TargetLanguage;
        SourceLanguage = input.SourceLanguage;
        AdaptiveDatasetName = input.AdaptiveDatasetName;
        GlossaryName = input.GlossaryName;
        IgnoreGlossaryCase = input.IgnoreGlossaryCase;
        Text = string.Empty;
        MimeType = "text/html";
    }

    public TextTranslationRequest()
    {
        Text = string.Empty;
        MimeType = "text/html";
    }
}