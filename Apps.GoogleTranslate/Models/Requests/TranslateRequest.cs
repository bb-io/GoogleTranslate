using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public class TranslateRequest
{
    public string Content { get; set; }

    [Display("Target language code"), DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; }
}