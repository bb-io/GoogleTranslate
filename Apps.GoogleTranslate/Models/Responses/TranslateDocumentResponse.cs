using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.GoogleTranslate.Models.Responses
{
    public class TranslateDocumentResponse
    {
        public File File { get; set; }

        [Display("Detected source language")]
        public string DetectedSourceLanguage { get; set; }
    }
}
