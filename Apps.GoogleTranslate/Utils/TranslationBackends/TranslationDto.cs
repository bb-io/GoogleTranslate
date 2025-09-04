namespace Apps.GoogleTranslate.Utils.TranslationBackends;

public record TranslationDto(
    string TranslatedText,
    string? DetectedSourceLanguage = null
);
