using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Api.Gax.ResourceNames;
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
        var projectId = Client.LocationName.ProjectId;
        var location = new LocationName(projectId, "us-central1");
        var request = new ListModelsRequest
        {
            ParentAsLocationName = location,
        };

        var models = Client.TranslateClient.ListModelsAsync(request);
        var resultingData = new List<DataSourceItem>();

        await foreach (var model in models)
        {
            if (context.SearchString != null && !GetModelDisplayName(model).Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                continue;

            resultingData.Add(new DataSourceItem(model.Name, GetModelDisplayName(model)));
        }

        return resultingData;
    }
    
    private string GetModelDisplayName(Model model)
    {
        return $"({model.SourceLanguageCode}-{model.TargetLanguageCode}) {model.DisplayName}";
    }
}