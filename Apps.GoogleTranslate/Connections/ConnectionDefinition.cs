using Apps.GoogleTranslate.Constants;
using Apps.GoogleTranslate.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Newtonsoft.Json;

namespace Apps.GoogleTranslate.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Service account",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredNames.ServiceAccountConfigurationString) { DisplayName = "Service account configuration string" },
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
    {
        var serviceAccountConfString = values.First(v => v.Key == CredNames.ServiceAccountConfigurationString);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            serviceAccountConfString.Key,
            serviceAccountConfString.Value
        );

        var configurationString = JsonConvert.DeserializeObject<ConfigurationString>(serviceAccountConfString.Value) ??
                                  throw new Exception($"Invalid service account configuration string: {serviceAccountConfString.Value}");
        
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            CredNames.ProjectId,
            configurationString.ProjectId
        );
    }
}