using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.GoogleTranslate.Models.Responses;

public class ContentTranslationResponse : ITranslateFileOutput
{
    public FileReference File { get; set; } = default!;

    [Display("Detected source language")]
    public string DetectedSourceLanguage { get; set; } = string.Empty;
}
