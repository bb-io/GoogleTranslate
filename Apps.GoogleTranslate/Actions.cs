using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Cloud.Translate.V3;
using TranslateDocumentResponse = Apps.GoogleTranslate.Models.Responses.TranslateDocumentResponse;
using TranslateDocumentRequest = Apps.GoogleTranslate.Models.Requests.TranslateDocumentRequest;
using Google.Protobuf;
using Apps.GoogleTranslate.Dtos;
using Blackbird.Applications.Sdk.Common.Actions;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.GoogleTranslate
{
    [ActionList]
    public class Actions
    {
        [Action("Translate to language", Description = "Translate to specified language")]
        public TranslateResponse Translate(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] TranslateRequest input)
        {
            var client = new BlackbirdGoogleTranslateClient(authenticationCredentialsProviders);
            var request = new TranslateTextRequest
            {
                Contents = { input.Content },
                TargetLanguageCode = input.TargetLanguageCode,
                Parent = client.ProjectName.ToString()
            };
            TranslateTextResponse response = client.TranslateClient.TranslateText(request);
            Translation translation = response.Translations[0];
            return new TranslateResponse()
            {
                Translation = translation.TranslatedText,
                DetectedSourceLanguage = translation.DetectedLanguageCode
            };
        }

        [Action("Detect language", Description = "Detect language from string")]
        public DetectResponse DetectLanguage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] DetectRequest input)
        {
            var client = new BlackbirdGoogleTranslateClient(authenticationCredentialsProviders);
            var request = new DetectLanguageRequest
            {
                Content = input.Content,
                Parent = client.ProjectName.ToString()
            };
            var response = client.TranslateClient.DetectLanguage(request).Languages[0].LanguageCode;
            return new DetectResponse()
            {
                LanguageCode = response
            };
        }

        [Action("Translate document", Description = "Translate document (file)")]
        public TranslateDocumentResponse TranslateDocumentLanguage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] TranslateDocumentRequest input)
        {
            var client = new BlackbirdGoogleTranslateClient(authenticationCredentialsProviders);

            DocumentInputConfig config;
            using (MemoryStream stream = new MemoryStream(input.File.Bytes))
            {
                config = new DocumentInputConfig
                {
                    Content = ByteString.FromStream(stream),
                    MimeType = input.File.ContentType
                };
            }

            var request = new Google.Cloud.Translate.V3.TranslateDocumentRequest
            {
                DocumentInputConfig = config,
                TargetLanguageCode = input.TargetLanguageCode,
                Parent = client.LocationName.ToString()
            };
            var response = client.TranslateClient.TranslateDocument(request);
            
            var translatedFileBytes = response.DocumentTranslation.ByteStreamOutputs[0].ToByteArray();
            var dotIndex = input.File.Name.LastIndexOf(".");
            dotIndex = dotIndex == -1 ? input.File.Name.Length : dotIndex;
            var translatedFileName = input.File.Name.Insert(dotIndex, $"_{input.TargetLanguageCode}");
            
            return new TranslateDocumentResponse
            {
                File = new File(translatedFileBytes)
                {
                    ContentType = response.DocumentTranslation.MimeType,
                    Name = translatedFileName
                },
                DetectedSourceLanguage = response.DocumentTranslation.DetectedLanguageCode
            };
        }

        [Action("Get supported languages", Description = "Get supported languages")]
        public GetSupportedLanguagesResponse GetSupportedLanguages(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new BlackbirdGoogleTranslateClient(authenticationCredentialsProviders);
            var request = new GetSupportedLanguagesRequest
            {
                Parent = client.LocationName.ToString()
            };
            var response = client.TranslateClient.GetSupportedLanguages(request).Languages.Select(l => new LanguageDto() { LanguageCode = l.LanguageCode}).ToList();
            return new GetSupportedLanguagesResponse()
            {
                SupportedLanguages = response
            };
        }
    }
}
