using Apps.GoogleTranslate.Api;
using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.GoogleTranslate.Utils.TranslationBackends;

public static class TranslationBackendFactory
{
    public static ITranslationBackend GetBackend(BaseGoogleTranslationRequest config)
    {
        if (!string.IsNullOrEmpty(config.CustomModelName))
            return new ModelTranslationBackend();

        if (!string.IsNullOrEmpty(config.AdaptiveDatasetName))
            return new AdaptiveTranslationBackend();

        if (!string.IsNullOrEmpty(config.TargetLanguage))
            return new GenericTranslationBackend();

        throw new PluginMisconfigurationException("Either target language, dataset or custom model are required.");
    }

    public static async Task<IEnumerable<TranslationDto>> TranslateTextAsync(
        IEnumerable<string> texts,
        BaseGoogleTranslationRequest config,
        BlackbirdGoogleTranslateClient client)
    {
        return await GetBackend(config)
            .TranslateTextAsync(texts, config, client);
    }

    public static async Task<ContentTranslationResponse> TranslateFileAsync(
        FileReference inputFile,
        BaseGoogleTranslationRequest config,
        BlackbirdGoogleTranslateClient client,
        IFileManagementClient fileManagementClient)
    {
        return await GetBackend(config)
            .TranslateFileAsync(inputFile, config, client, fileManagementClient);
    }
}