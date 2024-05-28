using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class GlossaryResponse
{
    [Display("Display name")]
    public string GlossaryName { get; set; }
    
    [Display("Full name")]
    public string FullName { get; set; }

    [Display("Entry count")]
    public int EntryCount { get; set; }

    [Display("Submit time")]
    public DateTime SubmitTime { get; set; }
    
    [Display("End time")]
    public DateTime EndTime { get; set; }

    [Display("Source language")]
    public string SourceLanguage { get; set; }
    
    [Display("Target language")]
    public string TargetLanguage { get; set; }
}