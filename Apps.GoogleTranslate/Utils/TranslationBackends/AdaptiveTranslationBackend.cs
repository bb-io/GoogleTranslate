using Apps.GoogleTranslate.Api;
using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.Utils.TranslationBackends;

public class AdaptiveTranslationBackend : ITranslationBackend
{
    public async Task<IEnumerable<TranslationDto>> TranslateTextAsync(
        IEnumerable<string> texts,
        string _,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client)
    {
        var translations = new List<TranslationDto>();

        foreach (var text in texts)
        {
            var adaptiveRequest = new AdaptiveMtTranslateRequest
            {
                Content = { text },
                Parent = client.LocationName.ToString().Replace("/global", "/us-central1"),
                Dataset = config.AdaptiveDatasetName,
            };

            if (!string.IsNullOrEmpty(config.GlossaryName))
            {
                adaptiveRequest.GlossaryConfig = new AdaptiveMtTranslateRequest.Types.GlossaryConfig
                {
                    Glossary = config.GlossaryName,
                    IgnoreCase = config.IgnoreGlossaryCase ?? true
                };
            }

            var adaptiveMtTranslationResponse = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await client.TranslateClient.AdaptiveMtTranslateAsync(adaptiveRequest));

            var receivedTranslations = string.IsNullOrEmpty(config.GlossaryName)
                ? adaptiveMtTranslationResponse.Translations
                : adaptiveMtTranslationResponse.GlossaryTranslations;

            translations.AddRange(receivedTranslations.Select(t => new TranslationDto(t.TranslatedText)));
        }

        return translations;
    }

    public Task<ContentTranslationResponse> TranslateFileAsync(
        FileReference inputFile,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client,
        IFileManagementClient fileManagementClient)
    {
        throw new PluginMisconfigurationException("Native file translation is not supported with Google Adaptive Translation.");
    }
}