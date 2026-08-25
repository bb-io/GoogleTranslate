using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class CustomModelResponse
{
    [Display("Model name")]
    public string Name { get; set; } = string.Empty;

    [Display("Display name")]
    public string DisplayName { get; set; } = string.Empty;

    [Display("Dataset name")]
    public string DatasetName { get; set; } = string.Empty;

    [Display("Source language code")]
    public string SourceLanguageCode { get; set; } = string.Empty;

    [Display("Target language code")]
    public string TargetLanguageCode { get; set; } = string.Empty;

    [Display("Training example count")]
    public int TrainingExampleCount { get; set; }

    [Display("Validation example count")]
    public int ValidationExampleCount { get; set; }

    [Display("Test example count")]
    public int TestExampleCount { get; set; }

    [Display("Created at")]
    public DateTime? CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime? UpdatedAt { get; set; }
}
