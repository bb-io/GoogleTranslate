using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GoogleTranslate.Models.Responses
{
    public class TranslateDocumentResponse
    {
        public FileReference File { get; set; }

        [Display("Detected source language")]
        public string DetectedSourceLanguage { get; set; }
    }
}
