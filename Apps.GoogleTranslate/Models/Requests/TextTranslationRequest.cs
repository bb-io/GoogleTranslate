using Apps.GoogleTranslate.DataSourceHandlers;
using Apps.GoogleTranslate.DataSourceHandlers.Enums;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.GoogleTranslate.Models.Requests;

public class TextTranslationRequest : ITranslateTextInput
{
    [Display("Text")]
    public string Text { get; set; }

    // Target language is required by ITranslateTextInput, so we cannot move to the BaseTranslationConfig
    [Display("Target language", Description = "Target language of translation, ignored when adaptive dataset is selected")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("MIME type", Description = "Type of the content")]
    [StaticDataSource(typeof(MimeTypeDataHandler))]
    public string? MimeType { get; set; } = "text/html";
}