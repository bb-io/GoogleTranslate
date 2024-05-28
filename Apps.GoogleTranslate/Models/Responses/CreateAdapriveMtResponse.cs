using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class CreateAdaptiveMtResponse
{
    [Display("Full name")]
    public string Name { get; set; }

    [Display("Display name")]
    public string DisplayName { get; set; }

    [Display("Entry count")]
    public int EntryCount { get; set; }
}