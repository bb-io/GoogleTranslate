using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class GlossaryDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var projectId = Client.LocationName.ProjectId;
        var location = new LocationName(projectId, "us-central1");
        var request = new ListGlossariesRequest
        {
            ParentAsLocationName = location
        };

        var glossaries = Client.TranslateClient.ListGlossariesAsync(request);
        var response = new List<DataSourceItem>();

        await foreach (var glossary in glossaries)
        {
            if (context.SearchString != null && !glossary.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                continue;

            response.Add(new DataSourceItem(glossary.Name, glossary.DisplayName));
        }

        return response;
    }
}