using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class AdaptiveDatasetDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var projectId = Client.LocationName.ProjectId;
        var location = new LocationName(projectId, "us-central1");
        var request = new ListAdaptiveMtDatasetsRequest
        {
            ParentAsLocationName = location
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