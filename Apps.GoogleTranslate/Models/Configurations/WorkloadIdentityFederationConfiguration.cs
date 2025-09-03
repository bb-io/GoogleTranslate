using Newtonsoft.Json;

namespace Apps.GoogleTranslate.Models.Configurations;

public class WorkloadIdentityFederationConfiguration : IConnectionConfiguration
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("universe_domain")]
    public string UniverseDomain { get; set; } = string.Empty;

    [JsonProperty("audience")]
    public string Audience { get; set; } = string.Empty;

    [JsonProperty("subject_token_type")]
    public string SubjectTokenType { get; set; } = string.Empty;

    [JsonProperty("token_url")]
    public string TokenUrl { get; set; } = string.Empty;

    [JsonProperty("service_account_impersonation_url")]
    public string ServiceAccountImpersonationUrl { get; set; } = string.Empty;

    [JsonProperty("credential_source")]
    public CredentialSource CredentialSource { get; set; } = new();

    public string GetProjectId() => Audience.Split('/')[4];

    public string GetLocationId() => Audience.Split('/')[6];

    public string ToJson()
    {
        var copy = (WorkloadIdentityFederationConfiguration)this.MemberwiseClone();

        // Override the CredentialSource to use Blackbird's token exchange endpoint
        copy.CredentialSource = new()
        {
            Url = "https://bridge.blackbird.io/api/azure-app-access-token",
            Headers = new Dictionary<string, string>
            {
                {"Blackbird-Token", ApplicationConstants.BlackbirdToken},
            },
            Format = new CredentialFormat
            {
                Type = "json",
                SubjectTokenFieldName = "access_token",
            }
        };

        return JsonConvert.SerializeObject(copy, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}

public class CredentialSource
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];

    [JsonProperty("format")]
    public CredentialFormat Format { get; set; } = new();
}

public class CredentialFormat
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("subject_token_field_name")]
    public string SubjectTokenFieldName { get; set; } = string.Empty;
}
