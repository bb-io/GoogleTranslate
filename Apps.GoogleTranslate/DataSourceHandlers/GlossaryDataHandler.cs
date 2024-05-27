using Apps.GoogleTranslate.BlackbirdActions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class GlossaryDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var actions = new GlossaryActions(InvocationContext);

        var languages = await actions.GetAllGlossaries();
        return languages.Glossaries
            .Where(x => context.SearchString == null || x.GlossaryName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.FullName, x => x.GlossaryName);
    }
}