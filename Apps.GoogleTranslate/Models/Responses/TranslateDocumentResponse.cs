using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleTranslate.Models.Responses
{
    public class TranslateDocumentResponse
    {
        public byte[] File { get; set; }

        public string MimeType { get; set; }

        public string DetectedSourceLanguage { get; set; }
    }
}
