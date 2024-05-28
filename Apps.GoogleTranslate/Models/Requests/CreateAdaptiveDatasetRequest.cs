using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GoogleTranslate.Models.Requests;

public class CreateAdaptiveDatasetRequest
{
    [Display("Adaptive MT dataset name")]
    public string Name { get; set; }
    
    [Display("Source language code", Description = "The source language code. Example: en")]
    [DataSource(typeof(LanguageDataHandler))]
    public string SourceLanguageCode { get; set; }

    [Display("Target language code", Description = "The target language code. Example: fr")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; }
    
    [Display("GCS input source URL", Description = "The GCS input source URL. Example: gs://bucket_name/path/to/input_file.tmx. Use either GCS input source or input file.")]
    public string? GcsInputSource { get; set; }

    [Display("Input file", Description = "The input file. Supported file types: .tmx, .tsv, .csv. Use either GCS input source or input file.")]
    public FileReference? File { get; set; }

    [Display("Example count", Description = "The number of examples in the dataset. Example: 1000")]
    public int? ExampleCount { get; set; }
}