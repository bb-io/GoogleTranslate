using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Requests
{
    public class TranslateRequest
    {
        public string Content { get; set; }

        [Display("Target language code")]
        public string TargetLanguageCode { get; set; }
    }
}
