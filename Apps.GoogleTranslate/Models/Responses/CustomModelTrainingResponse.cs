using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class CustomModelTrainingResponse
{
    [Display("Operation name")]
    public string OperationName { get; set; } = string.Empty;

    [Display("Status")]
    public string Status { get; set; } = string.Empty;

    [Display("Successful")]
    public bool IsSuccessful { get; set; }

    [Display("Error code")]
    public int? ErrorCode { get; set; }

    [Display("Error message")]
    public string? ErrorMessage { get; set; }

    [Display("Custom model")]
    public CustomModelResponse? CustomModel { get; set; }
}
