using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public class TranslateRequest
{
    [Display("Text")]
    public string Content { get; set; }

    [Display("Source language"), DataSource(typeof(LanguageDataHandler))]
    public string? SourceLanguage { get; set; }

    [Display("Target language", Description = "Specify if you want to translate without using dataset"), DataSource(typeof(LanguageDataHandler))]
    public string? TargetLanguageCode { get; set; }
    
    [Display("Adaptive dataset name", Description = "Specify if you want to translate using dataset"), DataSource(typeof(AdaptiveDatasetDataHandler))]
    public string? AdaptiveDatasetName { get; set; }
}