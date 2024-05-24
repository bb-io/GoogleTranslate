using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public class TranslateRequest
{
    [Display("Text")]
    public string Content { get; set; }

    [Display("Target language"), DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; }
}