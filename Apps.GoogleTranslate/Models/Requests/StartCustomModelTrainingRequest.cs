using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public class StartCustomModelTrainingRequest
{
    [Display("Dataset")]
    [DataSource(typeof(CustomDatasetDataHandler))]
    public string DatasetName { get; set; } = string.Empty;

    [Display("Model name", Description = "A display name containing up to 32 ASCII letters, digits, or underscores.")]
    public string ModelName { get; set; } = string.Empty;
}
