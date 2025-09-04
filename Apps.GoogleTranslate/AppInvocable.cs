using Apps.GoogleTranslate.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GoogleTranslate;

public class AppInvocable(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected BlackbirdGoogleTranslateClient Client { get; } = new(invocationContext.AuthenticationCredentialsProviders);
}