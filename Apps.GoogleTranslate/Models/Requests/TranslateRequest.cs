using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleTranslate.Models.Requests
{
    public class TranslateRequest
    {
        public string Content { get; set; }

        public string TargetLanguageCode { get; set; }
    }
}
