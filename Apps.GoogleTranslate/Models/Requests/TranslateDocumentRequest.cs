using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleTranslate.Models.Requests
{
    public class TranslateDocumentRequest
    {
        public byte[] File { get; set; }

        public string MimeType { get; set; }

        public string TargetLanguageCode { get; set; }
    }
}
