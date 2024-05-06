using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Dtos;

public class LanguageDto
{
    [Display("Language code")]
    public string LanguageCode { get; set; }
}