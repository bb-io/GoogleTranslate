using Newtonsoft.Json;

namespace Apps.GoogleTranslate.Models.Configurations;

public class ServiceAccountConfiguration : IConnectionConfiguration
{
    [JsonProperty("type")]
    public string Type { get; set; } = "service_account";
    
    [JsonProperty("project_id")]
    public string? ProjectId { get; set; }
    
    [JsonProperty("private_key_id")]
    public string? PrivateKeyId { get; set; }
    
    [JsonProperty("private_key")]
    public string? PrivateKey { get; set; }
    
    [JsonProperty("client_email")]
    public string? ClientEmail { get; set; }
    
    [JsonProperty("client_id")]
    public string? ClientId { get; set; }
    
    [JsonProperty("auth_uri")]
    public string? AuthUri { get; set; }
    
    [JsonProperty("token_uri")]
    public string? TokenUri { get; set; }
    
    [JsonProperty("auth_provider_x509_cert_url")]
    public string? AuthProviderX509CertUrl { get; set; }
    
    [JsonProperty("client_x509_cert_url")]
    public string? ClientX509CertUrl { get; set; }

    [JsonProperty("universe_domain")]
    public string? UniverseDomain { get; set; }

    public string GetProjectId() => ProjectId ?? string.Empty;

    public string GetLocationId() => "global";

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}