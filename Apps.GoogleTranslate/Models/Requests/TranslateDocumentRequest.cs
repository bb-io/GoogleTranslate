using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GoogleTranslate.Models.Requests;

public class TranslateDocumentRequest
{
    public FileReference File { get; set; }

    [Display("Target language"), DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; }
}