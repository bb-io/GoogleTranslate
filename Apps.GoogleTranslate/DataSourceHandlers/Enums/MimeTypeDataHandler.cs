using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.DataSourceHandlers.Enums;

public class MimeTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("text/plain", "Plain text"),
        new("text/html", "HTML"),
    ];
}
