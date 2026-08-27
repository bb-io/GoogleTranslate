using Apps.GoogleTranslate.DataSourceHandlers;
using Apps.GoogleTranslate.DataSourceHandlers.Enums;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GoogleTranslate.Models.Requests;

public class CreateCustomDatasetRequest
{
    [Display("Dataset name", Description = "A display name containing up to 32 ASCII letters, digits, or underscores.")]
    public string Name { get; set; } = string.Empty;

    [Display("Source language code", Description = "The BCP-47 source language code. Example: en.")]
    [DataSource(typeof(LanguageDataHandler))]
    public string SourceLanguageCode { get; set; } = string.Empty;

    [Display("Target language code", Description = "The BCP-47 target language code. Example: fr.")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; } = string.Empty;

    [Display(
        "GCS input source URL",
        Description = "Optional URI of an existing .tsv or .tmx file in Google Cloud Storage. Use either this input or Input file.")]
    public string? GcsInputSource { get; set; }

    [Display(
        "Input file",
        Description = "Optional .tsv or .tmx file to upload to Google Cloud Storage and import. Use either this input or GCS input source URL.")]
    public FileReference? File { get; set; }

    [Display(
        "File usage",
        Description = "How the imported file will be used. Defaults to Unassigned, which lets Google split the data automatically.")]
    [StaticDataSource(typeof(DatasetUsageDataHandler))]
    public string? Usage { get; set; }

    [Display(
        "GCS bucket name",
        Description = "Google Cloud Storage bucket where Input file will be uploaded. Required when Input file is specified. The bucket must be in us-central1.")]
    public string? GcsBucketName { get; set; }

    [Display(
        "GCS folder path",
        Description = "Optional folder path within the GCS bucket where Input file will be uploaded. Do not include the bucket name or gs://.")]
    public string? GcsFolderPath { get; set; }
}
