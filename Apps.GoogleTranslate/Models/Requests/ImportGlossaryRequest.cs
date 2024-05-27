using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public class ImportGlossaryRequest
{
    [Display("Glossary name")]
    public string Name { get; set; }
    
    [Display("GCS input source URL", Description = "The Google cloud storage input source URL. Example: gs://bucket_name/path/to/input_file.tmx. Use either GCS input source or input file.")]
    public string GcsInputSource { get; set; }

    [Display("Source language code", Description = "The source language code. Example: en")]
    [DataSource(typeof(LanguageDataHandler))]
    public string SourceLanguageCode { get; set; }

    [Display("Target language code", Description = "The target language code. Example: fr")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; }
}