using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.GoogleTranslate.Models.Requests
{
    public class TranslateDocumentRequest
    {
        public File File { get; set; }

        [Display("Target language code")]
        public string TargetLanguageCode { get; set; }
    }
}
