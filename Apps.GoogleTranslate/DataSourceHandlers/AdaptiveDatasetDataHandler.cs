using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class AdaptiveDatasetDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new ListAdaptiveMtDatasetsRequest
        {
            Parent = Client.LocationName.ToString().Replace("/global", "/us-central1"),
        };

        var adaptiveDatasets = Client.TranslateClient.ListAdaptiveMtDatasetsAsync(request);
        var resultingData = new List<DataSourceItem>();

        await foreach (var adaptiveDataset in adaptiveDatasets)
        {
            if (context.SearchString != null && !adaptiveDataset.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                continue;

            resultingData.Add(new DataSourceItem(adaptiveDataset.Name, adaptiveDataset.DisplayName));
        }

        return resultingData;
    }
}