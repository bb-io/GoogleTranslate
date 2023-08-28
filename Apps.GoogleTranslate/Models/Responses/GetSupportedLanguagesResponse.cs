using Apps.GoogleTranslate.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses
{
    public class GetSupportedLanguagesResponse
    {
        [Display("Supported languages")]
        public IEnumerable<LanguageDto> SupportedLanguages { get; set; }
    }
}
