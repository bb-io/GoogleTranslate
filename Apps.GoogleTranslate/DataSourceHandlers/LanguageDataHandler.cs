using Apps.GoogleTranslate.Utils;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class LanguageDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var projectId = Client.LocationName.ProjectId;
        var location = new LocationName(projectId, "us-central1");
        var request = new GetSupportedLanguagesRequest
        {
            ParentAsLocationName = location,
            DisplayLanguageCode = "en"
        };

        var response = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => 
            await Client.TranslateClient.GetSupportedLanguagesAsync(request));

        return response.Languages
            .Where(l => context.SearchString == null || l.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(l => new DataSourceItem(l.LanguageCode, l.DisplayName));
    }
}
