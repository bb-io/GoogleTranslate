using Apps.GoogleTranslate.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleTranslate.Models.Responses
{
    public class GetSupportedLanguagesResponse
    {
        public IEnumerable<LanguageDto> SupportedLanguages { get; set; }
    }
}
