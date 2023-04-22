using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleTranslate.Connections
{
    public class ConnectionDefinition : IConnectionDefinition
    {
        public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
        {
            new ConnectionPropertyGroup
            {
                Name = "Service account",
                AuthenticationType = ConnectionAuthenticationType.Undefined,
                ConnectionUsage = ConnectionUsage.Actions,
                ConnectionProperties = new List<ConnectionProperty>()
                {
                    new ConnectionProperty("serviceAccountConfString"),
                    new ConnectionProperty("projectId")
                }
            }
        };

        public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
        {
            var serviceAccountConfString = values.First(v => v.Key == "serviceAccountConfString");
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.None,
                serviceAccountConfString.Key,
                serviceAccountConfString.Value
            );

            var projectId = values.First(v => v.Key == "projectId");
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.None,
                projectId.Key,
                projectId.Value
            );
        }
    }
}
