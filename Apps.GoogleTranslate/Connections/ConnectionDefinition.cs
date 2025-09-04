using Apps.GoogleTranslate.Constants;
using Apps.GoogleTranslate.Models.Configurations;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.GoogleTranslate.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Client configuration",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredNames.ClientConfiguration) { DisplayName = "Client configuration JSON string" },
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
    {
        var configurationString = values.First(v => v.Key == CredNames.ClientConfiguration);

        var jObject = JObject.Parse(configurationString.Value);
        var connectionType = jObject.Value<string>("type") ?? string.Empty;

        IConnectionConfiguration configuration = connectionType switch
        {
            "service_account" => JsonConvert.DeserializeObject<ServiceAccountConfiguration>(configurationString.Value)
                                  ?? throw new PluginMisconfigurationException($"Unsupported service account configuration. Please, provide copy of the JSON file downloaded from Google Cloud Platform."),
            "external_account" => JsonConvert.DeserializeObject<WorkloadIdentityFederationConfiguration>(configurationString.Value)
                                  ?? throw new PluginMisconfigurationException($"Unsupported external account configuration string. Please, provide copy of the JSON file downloaded from Google Cloud Platform."),
            _ => throw new PluginMisconfigurationException($"Unsupported configuration type: {connectionType}")
        };

        yield return new AuthenticationCredentialsProvider(
            CredNames.ClientConfiguration,
            configuration.ToJson()
        );

        yield return new AuthenticationCredentialsProvider(
            CredNames.ProjectId,
            configuration.GetProjectId()
        );

        yield return new AuthenticationCredentialsProvider(
            CredNames.LocationId,
            configuration.GetLocationId()
        );
    }
}