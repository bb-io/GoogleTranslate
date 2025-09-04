using Apps.GoogleTranslate.Api;
using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.GoogleTranslate.Utils.TranslationBackends;

public interface ITranslationBackend
{
    Task<IEnumerable<TranslationDto>> TranslateTextAsync(
        IEnumerable<string> texts,
        string mimeType,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client);

    Task<ContentTranslationResponse> TranslateFileAsync(
        FileReference inputFile,
        BaseTranslationConfig config,
        BlackbirdGoogleTranslateClient client,
        IFileManagementClient fileManagementClient);
}