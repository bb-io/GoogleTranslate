using Apps.GoogleTranslate.Api;
using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.GoogleTranslate.Utils.TranslationBackends;

public static class TranslationBackendFactory
{
    public static ITranslationBackend GetBackend(BaseTranslationConfig config, string? targetLanguage = null)
    {
        if (!string.IsNullOrEmpty(config.CustomModelName))
            return new ModelTranslationBackend(targetLanguage);

        if (!string.IsNullOrEmpty(config.AdaptiveDatasetName))
            return new AdaptiveTranslationBackend();

        if (!string.IsNullOrEmpty(targetLanguage))
            return new GenericTranslationBackend(targetLanguage);

        throw new PluginMisconfigurationException("Either target language, dataset or custom model are required.");
    }

    public static async Task<IEnumerable<TranslationDto>> TranslateTextAsync(
        IEnumerable<string> texts,
        string mimeType,
        string targetLanguage,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client)
    {
        return await GetBackend(config, targetLanguage)
            .TranslateTextAsync(texts, mimeType, config, client);
    }

    public static async Task<ContentTranslationResponse> TranslateFileAsync(
        FileReference inputFile,
        string targetLanguage,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client,
        IFileManagementClient fileManagementClient)
    {
        return await GetBackend(config, targetLanguage)
            .TranslateFileAsync(inputFile, config, client, fileManagementClient);
    }
}