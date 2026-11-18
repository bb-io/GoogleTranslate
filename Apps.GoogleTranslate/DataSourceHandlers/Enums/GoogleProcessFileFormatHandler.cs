using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Filters.Constants;

namespace Apps.GoogleTranslate.DataSourceHandlers.Enums;

public class GoogleProcessFileFormatHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new(MediaTypes.Xliff2, "Interoperable XLIFF (default)"),
        new("xliff1", "XLIFF 1.2"),
        new("original", "Original data format")
    ];
}