using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class CustomDatasetDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(
        DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var location = new LocationName(Client.LocationName.ProjectId, "us-central1");
        var datasets = Client.TranslateClient.ListDatasetsAsync(new ListDatasetsRequest
        {
            ParentAsLocationName = location
        });

        var result = new List<DataSourceItem>();
        await foreach (var dataset in datasets.WithCancellation(cancellationToken))
        {
            var displayName = $"({dataset.SourceLanguageCode}-{dataset.TargetLanguageCode}) {dataset.DisplayName}";
            if (context.SearchString is not null &&
                !displayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            result.Add(new DataSourceItem(dataset.Name, displayName));
        }

        return result;
    }
}
