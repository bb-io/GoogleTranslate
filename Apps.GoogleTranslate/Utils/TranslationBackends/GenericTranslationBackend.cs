using Apps.GoogleTranslate.Api;
using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Google.Cloud.Translate.V3;
using Google.Protobuf;

namespace Apps.GoogleTranslate.Utils.TranslationBackends;

public class GenericTranslationBackend : ITranslationBackend
{
    public static void ValidateConfig(BaseGoogleTranslationRequest config)
    {
        if (string.IsNullOrWhiteSpace(config.TargetLanguage))
            throw new PluginMisconfigurationException("The target language can not be empty, please fill the 'Target language' field and make sure it has a valid language code");

        if (string.IsNullOrWhiteSpace(config.SourceLanguage) != string.IsNullOrWhiteSpace(config.GlossaryName))
            throw new PluginMisconfigurationException("When using a glossary, the source language must be specified.");
    }

    public async Task<IEnumerable<TranslationDto>> TranslateTextAsync(
        IEnumerable<string> texts,
        BaseGoogleTranslationRequest config,
        BlackbirdGoogleTranslateClient client)
    {
        ValidateConfig(config);

        var genericRequest = new TranslateTextRequest
        {
            Contents = { texts },
            TargetLanguageCode = config.TargetLanguage,
            SourceLanguageCode = config.SourceLanguage ?? string.Empty,
            Parent = client.LocationName.ToString(),
        };

        if (!string.IsNullOrEmpty(config.GlossaryName))
        {
            genericRequest.GlossaryConfig = new TranslateTextGlossaryConfig
            {
                Glossary = config.GlossaryName,
                IgnoreCase = config.IgnoreGlossaryCase ?? true
            };
        }

        var response = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await client.TranslateClient.TranslateTextAsync(genericRequest));

        var translations = string.IsNullOrEmpty(config.GlossaryName)
            ? response.Translations
            : response.GlossaryTranslations;

        return translations.Select(t => new TranslationDto(t.TranslatedText, t.DetectedLanguageCode));
    }

    public async Task<ContentTranslationResponse> TranslateFileAsync(
        FileReference inputFile,
        BaseGoogleTranslationRequest config,
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
            TargetLanguageCode = config.TargetLanguage,
            Parent = client.LocationName.ToString(),
            IsTranslateNativePdfOnly = inputFile.ContentType.Equals("application/pdf", System.StringComparison.InvariantCultureIgnoreCase)
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