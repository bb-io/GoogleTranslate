using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GoogleTranslate.DataSourceHandlers;

public class LanguageDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var actions = new Actions(invocationContext, null);

        var languages = await actions.GetSupportedLanguages();
        return languages.SupportedLanguages
            .Where(x => context.SearchString == null || x.LanguageName.Contains(context.SearchString))
            .ToDictionary(x => x.LanguageCode, x => x.LanguageName);
    }
}