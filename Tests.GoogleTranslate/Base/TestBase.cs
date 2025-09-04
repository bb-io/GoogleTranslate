using Apps.GoogleTranslate.Connections;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Extensions.Configuration;

namespace Tests.GoogleTranslate.Base;

public class TestBase
{
    public IEnumerable<AuthenticationCredentialsProvider> Creds { get; set; }

    public InvocationContext InvocationContext { get; set; }

    public FileManager FileManager { get; set; }

    public TestBase()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var appConnection = new ConnectionDefinition();

        Creds = appConnection.CreateAuthorizationCredentialsProviders(
            config.GetSection("ConnectionDefinition")
                .GetChildren()
                .ToDictionary(x => x.Key, x => x.Value ?? string.Empty)
        ).ToList();

        InvocationContext = new InvocationContext
        {
            AuthenticationCredentialsProviders = Creds,
        };

        FileManager = new FileManager();
    }
}
