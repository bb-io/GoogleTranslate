﻿using Apps.GoogleTranslate.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

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
                new(CredNames.ProjectId) { DisplayName = "Project ID" }
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

        var projectId = values.First(v => v.Key == CredNames.ProjectId);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            projectId.Key,
            projectId.Value
        );
    }
}