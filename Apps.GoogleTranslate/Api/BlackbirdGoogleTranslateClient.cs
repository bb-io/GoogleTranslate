using Apps.GoogleTranslate.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.Api;

public class BlackbirdGoogleTranslateClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
{
    private readonly string _serviceAccountConfString = authenticationCredentialsProviders.First(p => p.KeyName == CredNames.ClientConfiguration).Value;
    private readonly string _projectId = authenticationCredentialsProviders.First(p => p.KeyName == CredNames.ProjectId).Value;
    private readonly string _locationId = authenticationCredentialsProviders.First(p => p.KeyName == CredNames.LocationId).Value;

    public TranslationServiceClient TranslateClient => new TranslationServiceClientBuilder { JsonCredentials = _serviceAccountConfString }.Build();
    public ProjectName ProjectName => new(_projectId);
    public LocationName LocationName => new(_projectId, _locationId);
}