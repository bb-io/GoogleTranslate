using Apps.GoogleTranslate.Api;
using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Google.Cloud.Translate.V3;
using Google.Protobuf;

namespace Apps.GoogleTranslate.Utils.TranslationBackends;

public class ModelTranslationBackend(string targetLanguage) : ITranslationBackend
{
    public readonly string TargetLanguage = targetLanguage;

    public static void ValidateConfig(BaseTranslationConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.SourceLanguage))
            throw new PluginMisconfigurationException("The source language must be specified when using a custom model.");
    }

    public async Task<IEnumerable<TranslationDto>> TranslateTextAsync(
        IEnumerable<string> texts,
        string mimeType,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client)
    {
        ValidateConfig(config);

        var translations = new List<TranslationDto>();

        foreach (var text in texts)
        {
            var request = new TranslateTextRequest
            {
                Contents = { texts },
                TargetLanguageCode = TargetLanguage,
                SourceLanguageCode = config.SourceLanguage,
                Parent = client.LocationName.ToString().Replace("/global", "/us-central1"),
                Model = config.CustomModelName,
                MimeType = mimeType,
            };

            if (!string.IsNullOrEmpty(config.GlossaryName))
            {
                request.GlossaryConfig = new TranslateTextGlossaryConfig
                {
                    Glossary = config.GlossaryName,
                    IgnoreCase = config.IgnoreGlossaryCase ?? true
                };
            }

            var adaptiveMtTranslationResponse = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.TranslateClient.TranslateTextAsync(request));

            var receivedTranslations = string.IsNullOrEmpty(config.GlossaryName)
                ? adaptiveMtTranslationResponse.Translations
                : adaptiveMtTranslationResponse.GlossaryTranslations;

            translations.AddRange(receivedTranslations.Select(t => new TranslationDto(t.TranslatedText)));
        }

        return translations;
    }

    public async Task<ContentTranslationResponse> TranslateFileAsync(
        FileReference inputFile,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client,
        IFileManagementClient fileManagementClient)
    {
        ValidateConfig(config);

        var fileStream = await fileManagementClient.DownloadAsync(inputFile);
        var documentConfig = new DocumentInputConfig
        {
            Content = await ByteString.FromStreamAsync(fileStream),
            MimeType = inputFile.ContentType
        };

        var request = new TranslateDocumentRequest
        {
            DocumentInputConfig = documentConfig,
            TargetLanguageCode = TargetLanguage,
            SourceLanguageCode = config.SourceLanguage,
            Parent = client.LocationName.ToString().Replace("/global", "/us-central1"),
            IsTranslateNativePdfOnly = inputFile.ContentType.Equals("application/pdf", System.StringComparison.InvariantCultureIgnoreCase),
            Model = config.CustomModelName
        };

        if (!string.IsNullOrEmpty(config.GlossaryName))
        {
            request.GlossaryConfig = new TranslateTextGlossaryConfig
            {
                Glossary = config.GlossaryName,
                IgnoreCase = config.IgnoreGlossaryCase ?? true
            };
        }

        var response = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await client.TranslateClient.TranslateDocumentAsync(request));

        var translatedFileBytes = response.DocumentTranslation.ByteStreamOutputs[0].ToByteArray();
        using var stream = new System.IO.MemoryStream(translatedFileBytes);

        var translatedFile = await fileManagementClient
            .UploadAsync(stream, response.DocumentTranslation.MimeType, inputFile.Name);

        return new ContentTranslationResponse
        {
            File = translatedFile,
            DetectedSourceLanguage = response.DocumentTranslation.DetectedLanguageCode
        };
    }
}