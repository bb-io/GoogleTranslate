using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Requests;

public class CreateAdaptiveDatasetRequest
{
    [Display("Adaptive MT dataset name")]
    public string Name { get; set; }
    
    [Display("Source language code", Description = "The source language code. Example: en")]
    public string SourceLanguageCode { get; set; }

    [Display("Target language code", Description = "The target language code. Example: fr")]
    public string TargetLanguageCode { get; set; }
    
    [Display("GCS input source URL", Description = "The GCS input source URL. Example: gs://bucket_name/path/to/input_file.tmx")]
    public string GcsInputSource { get; set; }

    [Display("Example count", Description = "The number of examples in the dataset. Example: 1000")]
    public int? ExampleCount { get; set; }
}