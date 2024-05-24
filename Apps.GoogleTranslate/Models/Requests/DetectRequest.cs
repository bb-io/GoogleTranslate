using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Requests;

public class DetectRequest
{
    [Display("Text")]
    public string Content { get; set; }
}