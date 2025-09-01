using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.GoogleTranslate.Models.Responses;

public class TextTranslationResponse : ITranslateTextOutput
{
    [Display("Translated text", Description = "The text after translation")]
    public string TranslatedText { get; set; } = string.Empty;

    [Display("Detected source language")]
    public string? DetectedSourceLanguage { get; set; }
}