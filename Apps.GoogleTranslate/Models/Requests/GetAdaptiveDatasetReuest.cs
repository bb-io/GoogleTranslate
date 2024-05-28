using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public class GetAdaptiveDatasetRequest
{
    [Display("Adaptive dataset name", Description = "Example: projects/{project-id}/locations/{location-id}/datasets/{dataset-id}")]
    [DataSource(typeof(AdaptiveDatasetDataHandler))]
    public string AdaptiveDatasetName { get; set; }
}