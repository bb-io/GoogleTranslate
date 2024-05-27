using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.Models.Requests;

public class GlossaryRequest
{
    [Display("Glossary name"), DataSource(typeof(GlossaryDataHandler))]
    public string GlossaryName { get; set; }
}