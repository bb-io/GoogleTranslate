using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class AdaptiveMtResponse
{
    [Display("Full name")]
    public string Name { get; set; }
    
    [Display("Display name")]
    public string DisplayName { get; set; }
    
    [Display("Source language code")]
    public string SourceLanguageCode { get; set; }
    
    [Display("Target language code")]
    public string TargetLanguageCode { get; set; }
    
    [Display("Example count")]
    public int ExampleCount { get; set; }
}