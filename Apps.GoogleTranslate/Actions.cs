using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using TranslateDocumentResponse = Apps.GoogleTranslate.Models.Responses.TranslateDocumentResponse;
using TranslateDocumentRequest = Apps.GoogleTranslate.Models.Requests.TranslateDocumentRequest;
using Google.Protobuf;
using Apps.GoogleTranslate.Dtos;

namespace Apps.GoogleTranslate
{
    [ActionList]
    public class Actions
    {
        [Action("Translate to language", Description = "Translate to specified language")]
        public TranslateResponse Translate(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] TranslateRequest input)
        {
            var client = new TranslationServiceClientBuilder{ JsonCredentials = serviceAccountConfString }.Build();
            var request = new TranslateTextRequest
            {
                Contents = { input.Content },
                TargetLanguageCode = input.TargetLanguageCode,
                Parent = new ProjectName(authenticationCredentialsProvider.Value).ToString()
            };
            TranslateTextResponse response = client.TranslateText(request);
            Translation translation = response.Translations[0];
            return new TranslateResponse()
            {
                Translation = translation.TranslatedText,
                DetectedSourceLanguage = translation.DetectedLanguageCode
            };
        }

        [Action("Detect language", Description = "Detect language from string")]
        public DetectResponse DetectLanguage(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] DetectRequest input)
        {
            var client = new TranslationServiceClientBuilder { JsonCredentials = serviceAccountConfString }.Build();
            var request = new DetectLanguageRequest
            {
                Content = input.Content,
                Parent = new ProjectName(authenticationCredentialsProvider.Value).ToString()
            };
            var response = client.DetectLanguage(request).Languages[0].LanguageCode;
            return new DetectResponse()
            {
                LanguageCode = response
            };
        }

        [Action("Translate document", Description = "Translate document (file)")]
        public TranslateDocumentResponse TranslateDocumentLanguage(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] TranslateDocumentRequest input)
        {
            var client = new TranslationServiceClientBuilder { JsonCredentials = serviceAccountConfString }.Build();

            DocumentInputConfig config;
            using (MemoryStream stream = new MemoryStream(input.File))
            {
                config = new DocumentInputConfig()
                {
                    Content = ByteString.FromStream(stream),
                    MimeType = input.MimeType
                };
            }

            var request = new Google.Cloud.Translate.V3.TranslateDocumentRequest
            {
                DocumentInputConfig = config,
                TargetLanguageCode = input.TargetLanguageCode,
                Parent = new LocationName(authenticationCredentialsProvider.Value, "global").ToString()
            };
            var response = client.TranslateDocument(request);

            return new TranslateDocumentResponse()
            {
                File = response.DocumentTranslation.ByteStreamOutputs[0].ToByteArray(),
                DetectedSourceLanguage = response.DocumentTranslation.DetectedLanguageCode,
                MimeType = response.DocumentTranslation.MimeType,
            };
        }

        [Action("Get supported languages", Description = "Get supported languages")]
        public GetSupportedLanguagesResponse GetSupportedLanguages(string serviceAccountConfString, AuthenticationCredentialsProvider authenticationCredentialsProvider)
        {
            var client = new TranslationServiceClientBuilder { JsonCredentials = serviceAccountConfString }.Build();
            var request = new GetSupportedLanguagesRequest
            {
                Parent = new LocationName(authenticationCredentialsProvider.Value, "global").ToString()
            };
            var response = client.GetSupportedLanguages(request).Languages.Select(l => new LanguageDto() { LanguageCode = l.LanguageCode}).ToList();
            return new GetSupportedLanguagesResponse()
            {
                SupportedLanguages = response
            };
        }
    }
}
