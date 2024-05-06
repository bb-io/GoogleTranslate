using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class TranslateResponse
{
    public string Translation { get; set; }

    [Display("Detected source language")]
    public string DetectedSourceLanguage { get; set; }
}