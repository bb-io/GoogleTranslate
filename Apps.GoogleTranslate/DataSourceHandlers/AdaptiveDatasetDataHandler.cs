using Apps.GoogleTranslate.BlackbirdActions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class AdaptiveDatasetDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var actions = new AdaptiveDatasetActions(invocationContext, null);

        var languages = await actions.GetAdaptiveDatasetsAsync();
        return languages.AdaptiveMts
            .Where(x => context.SearchString == null || x.DisplayName.Contains(context.SearchString))
            .ToDictionary(x => x.Name, x => x.DisplayName);
    }
}