using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Google.Cloud.Translate.V3;
using TranslateDocumentResponse = Apps.GoogleTranslate.Models.Responses.TranslateDocumentResponse;
using TranslateDocumentRequest = Apps.GoogleTranslate.Models.Requests.TranslateDocumentRequest;
using Google.Protobuf;
using Apps.GoogleTranslate.Dtos;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.GoogleTranslate
{
    [ActionList]
    public class Actions : BaseInvocable
    {
        private readonly BlackbirdGoogleTranslateClient _client;
        private readonly IFileManagementClient _fileManagementClient;

        public Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
            : base(invocationContext)
        {
            _client = new(invocationContext.AuthenticationCredentialsProviders);
            _fileManagementClient = fileManagementClient;
        }
        
        [Action("Translate to language", Description = "Translate to specified language")]
        public TranslateResponse Translate([ActionParameter] TranslateRequest input)
        {
            var request = new TranslateTextRequest
            {
                Contents = { input.Content },
                TargetLanguageCode = input.TargetLanguageCode,
                Parent = _client.ProjectName.ToString()
            };
            TranslateTextResponse response = _client.TranslateClient.TranslateText(request);
            Translation translation = response.Translations[0];
            return new TranslateResponse()
            {
                Translation = translation.TranslatedText,
                DetectedSourceLanguage = translation.DetectedLanguageCode
            };
        }

        [Action("Detect language", Description = "Detect language from string")]
        public DetectResponse DetectLanguage([ActionParameter] DetectRequest input)
        {
            var request = new DetectLanguageRequest
            {
                Content = input.Content,
                Parent = _client.ProjectName.ToString()
            };
            var response = _client.TranslateClient.DetectLanguage(request).Languages[0].LanguageCode;
            return new DetectResponse()
            {
                LanguageCode = response
            };
        }

        [Action("Translate document", Description = "Translate document (file)")]
        public TranslateDocumentResponse TranslateDocumentLanguage([ActionParameter] TranslateDocumentRequest input)
        {
            var fileStream = _fileManagementClient.DownloadAsync(input.File).Result;
            var config = new DocumentInputConfig
            {
                Content = ByteString.FromStream(fileStream),
                MimeType = input.File.ContentType
            };

            var request = new Google.Cloud.Translate.V3.TranslateDocumentRequest
            {
                DocumentInputConfig = config,
                TargetLanguageCode = input.TargetLanguageCode,
                Parent = _client.LocationName.ToString()
            };
            var response = _client.TranslateClient.TranslateDocument(request);
            
            var translatedFileBytes = response.DocumentTranslation.ByteStreamOutputs[0].ToByteArray();
            var dotIndex = input.File.Name.LastIndexOf(".");
            dotIndex = dotIndex == -1 ? input.File.Name.Length : dotIndex;
            var translatedFileName = input.File.Name.Insert(dotIndex, $"_{input.TargetLanguageCode}");

            using var stream = new MemoryStream(translatedFileBytes);
            var translatedFile = _fileManagementClient.UploadAsync(stream, response.DocumentTranslation.MimeType,
                translatedFileName).Result;
            
            return new TranslateDocumentResponse
            {
                File = translatedFile,
                DetectedSourceLanguage = response.DocumentTranslation.DetectedLanguageCode
            };
        }

        [Action("Get supported languages", Description = "Get supported languages")]
        public GetSupportedLanguagesResponse GetSupportedLanguages()
        {
            var request = new GetSupportedLanguagesRequest
            {
                Parent = _client.LocationName.ToString()
            };
            var response = _client.TranslateClient.GetSupportedLanguages(request).Languages.Select(l => new LanguageDto() { LanguageCode = l.LanguageCode}).ToList();
            return new GetSupportedLanguagesResponse()
            {
                SupportedLanguages = response
            };
        }
    }
}
