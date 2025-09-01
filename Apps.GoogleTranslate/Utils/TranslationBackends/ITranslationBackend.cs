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
        BaseGoogleTranslationRequest config,
        BlackbirdGoogleTranslateClient client);

    Task<ContentTranslationResponse> TranslateFileAsync(
        FileReference inputFile,
        BaseGoogleTranslationRequest config,
        BlackbirdGoogleTranslateClient client,
        IFileManagementClient fileManagementClient);
}