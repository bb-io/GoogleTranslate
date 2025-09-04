using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.DataSourceHandlers;

/// <summary>
/// For AutoML custom models
/// https://cloud.google.com/translate/docs/advanced/translating-text-v3?#automl-model
/// </summary>
/// <param name="invocationContext"></param>
public class CustomModelDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(
        DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new ListModelsRequest
        {
            Parent = Client.LocationName.ToString().Replace("/global", "/us-central1"),
        };

        var models = Client.TranslateClient.ListModelsAsync(request);
        var resultingData = new List<DataSourceItem>();

        await foreach (var model in models)
        {
            if (context.SearchString != null && !model.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                continue;

            resultingData.Add(new DataSourceItem(model.Name, model.DisplayName));
        }

        return resultingData;
    }
}