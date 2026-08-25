using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Requests;

public class CustomModelTrainingIdentifier
{
    [Display("Operation name", Description = "The operation name returned by Start custom model training.")]
    public string OperationName { get; set; } = string.Empty;
}
