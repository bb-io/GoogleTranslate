namespace Apps.GoogleTranslate.Models.Configurations;

internal interface IConnectionConfiguration
{
    public string Type { get; set; } // e.g. "service_account" or "external_account"
    public string GetProjectId();
    public string GetLocationId();
    public string ToJson();
}
