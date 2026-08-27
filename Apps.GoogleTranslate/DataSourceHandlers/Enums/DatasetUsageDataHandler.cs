using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.DataSourceHandlers.Enums;

public class DatasetUsageDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("UNASSIGNED", "Unassigned (automatic split)"),
        new("TRAIN", "Training"),
        new("VALIDATION", "Validation"),
        new("TEST", "Test")
    ];
}
