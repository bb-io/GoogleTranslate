using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class StartCustomModelTrainingResponse
{
    [Display("Operation name")]
    public string OperationName { get; set; } = string.Empty;

    [Display("Status")]
    public string Status { get; set; } = string.Empty;
}
