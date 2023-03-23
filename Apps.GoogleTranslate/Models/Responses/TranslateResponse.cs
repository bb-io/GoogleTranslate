using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleTranslate.Models.Responses
{
    public class TranslateResponse
    {
        public string Translation { get; set; }

        public string DetectedSourceLanguage { get; set; }
    }
}
