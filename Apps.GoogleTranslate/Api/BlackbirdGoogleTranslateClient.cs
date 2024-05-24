using Apps.GoogleTranslate.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate;

public class BlackbirdGoogleTranslateClient
{
    private readonly string _serviceAccountConfString;
    private readonly string _projectId;

    public TranslationServiceClient TranslateClient => new TranslationServiceClientBuilder { JsonCredentials = _serviceAccountConfString }.Build();
    public ProjectName ProjectName => new ProjectName(_projectId);
    public LocationName LocationName => new LocationName(_projectId, "global");

    public BlackbirdGoogleTranslateClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        _serviceAccountConfString = authenticationCredentialsProviders.First(p => p.KeyName == CredNames.ServiceAccountConfigurationString).Value;
        _projectId = authenticationCredentialsProviders.First(p => p.KeyName == CredNames.ProjectId).Value;
    }
}