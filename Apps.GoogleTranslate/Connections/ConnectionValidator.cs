using Apps.GoogleTranslate.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
        try
        {
            var client = new BlackbirdGoogleTranslateClient(authProviders);
            var request = new DetectLanguageRequest
            {
                Content = "Test input for detecting language",
                Parent = client.ProjectName.ToString()
            };
            
            await client.TranslateClient.DetectLanguageAsync(request);

            return new()
            {
                IsValid = true
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
    }
}