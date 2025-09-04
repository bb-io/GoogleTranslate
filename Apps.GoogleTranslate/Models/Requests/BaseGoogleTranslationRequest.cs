using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public abstract class BaseGoogleTranslationRequest
{
    [Display("Target language", Description = "Target language of translation, ignored when adaptive dataset is selected")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Source language"), DataSource(typeof(LanguageDataHandler))]
    public string? SourceLanguage { get; set; }

    [Display("Adaptive dataset name", Description = "Specify if you want to translate using dataset")]
    [DataSource(typeof(AdaptiveDatasetDataHandler))]
    public string? AdaptiveDatasetName { get; set; }

    [Display("Custom AutoML model", Description = "Specify if you want to translate using custom AutoML model")]
    [DataSource(typeof(AdaptiveDatasetDataHandler))]
    public string? CustomModelName { get; set; }

    [Display("Glossary name", Description = "Specify if you want to use glossary for translation")]
    [DataSource(typeof(GlossaryDataHandler))]
    public string? GlossaryName { get; set; }

    [Display("Ignore case in glossary?", Description = "Specify if you want to ignore keys in glossary, default is true")]
    public bool? IgnoreGlossaryCase { get; set; }
}
