﻿using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleTranslate.DataSourceHandlers.Enums;

public class FileTranslationStrategyHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("blackbird", "Blackbird interoperable (default)"),
        new("native", "Google Translate native"),
    ];
}
