﻿using Apps.GoogleTranslate.Models.Requests;
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

namespace Apps.GoogleTranslate;

[ActionList]
public class Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Translate to language", Description = "Translate to specified language (using adaptive dataset if provided)")]
    public async Task<TranslateResponse> Translate([ActionParameter] TranslateRequest input)
    {
        if (string.IsNullOrEmpty(input.TargetLanguageCode) && string.IsNullOrEmpty(input.AdaptiveDatasetName))
        {
            throw new ArgumentException("Please provide either target language or adaptive dataset name. " +
                                        "If you want to translate without using dataset, please provide target language." +
                                        "If you want to translate using adaptive dataset, please provide adaptive dataset name. ");
        }

        if (!string.IsNullOrEmpty(input.TargetLanguageCode))
        {
            var request = new TranslateTextRequest
            {
                Contents = { input.Content },
                TargetLanguageCode = input.TargetLanguageCode,
                Parent = Client.ProjectName.ToString(),
                SourceLanguageCode = input.SourceLanguage,
                GlossaryConfig = string.IsNullOrEmpty(input.GlossaryName) ? null : new TranslateTextGlossaryConfig
                {
                    Glossary = input.GlossaryName,
                    IgnoreCase = input.IgnoreKeys ?? true
                }
            };

            var response = await Client.TranslateClient.TranslateTextAsync(request);
            var translation = string.IsNullOrEmpty(input.GlossaryName)
                ? response.Translations[0]
                : response.GlossaryTranslations.FirstOrDefault() ?? response.Translations[0];
            
            return new TranslateResponse
            {
                Translation = translation.TranslatedText,
                DetectedSourceLanguage = translation.DetectedLanguageCode
            };
        }

        var adaptiveMtTranslationResponse = await Client.TranslateClient.AdaptiveMtTranslateAsync(
            new AdaptiveMtTranslateRequest
            {
                Parent = Client.ProjectName + "/locations/us-central1",
                Dataset = input.AdaptiveDatasetName,
                Content = { input.Content }
            });

        return new TranslateResponse
        {
            Translation = adaptiveMtTranslationResponse.Translations.First().TranslatedText,
            DetectedSourceLanguage = adaptiveMtTranslationResponse.LanguageCode 
        };
    }

    [Action("Detect language", Description = "Detect language from string")]
    public async Task<DetectResponse> DetectLanguage([ActionParameter] DetectRequest input)
    {
        var request = new DetectLanguageRequest
        {
            Content = input.Content,
            Parent = Client.ProjectName.ToString()
        };
        
        var response = await Client.TranslateClient.DetectLanguageAsync(request);
        var language = response.Languages[0].LanguageCode;
        return new DetectResponse()
        {
            LanguageCode = language
        };
    }

    [Action("Translate document", Description = "Translate document (file)")]
    public async Task<TranslateDocumentResponse> TranslateDocumentLanguage(
        [ActionParameter] TranslateDocumentRequest input)
    {
        var fileStream = fileManagementClient.DownloadAsync(input.File).Result;
        var config = new DocumentInputConfig
        {
            Content = await ByteString.FromStreamAsync(fileStream),
            MimeType = input.File.ContentType
        };

        var request = new Google.Cloud.Translate.V3.TranslateDocumentRequest
        {
            DocumentInputConfig = config,
            TargetLanguageCode = input.TargetLanguageCode,
            Parent = Client.LocationName.ToString()
        };

        var response = await Client.TranslateClient.TranslateDocumentAsync(request);
        var translatedFileBytes = response.DocumentTranslation.ByteStreamOutputs[0].ToByteArray();

        var dotIndex = input.File.Name.LastIndexOf(".");
        dotIndex = dotIndex == -1 ? input.File.Name.Length : dotIndex;
        var translatedFileName = input.File.Name.Insert(dotIndex, $"_{input.TargetLanguageCode}");

        using var stream = new MemoryStream(translatedFileBytes);
        var translatedFile = fileManagementClient.UploadAsync(stream, response.DocumentTranslation.MimeType,
            translatedFileName).Result;

        return new TranslateDocumentResponse
        {
            File = translatedFile,
            DetectedSourceLanguage = response.DocumentTranslation.DetectedLanguageCode
        };
    }

    public async Task<GetSupportedLanguagesResponse> GetSupportedLanguages()
    {
        var request = new GetSupportedLanguagesRequest
        {
            Parent = Client.LocationName.ToString(),
            DisplayLanguageCode = "en"
        };

        var response = await Client.TranslateClient.GetSupportedLanguagesAsync(request);
        var languages = response.Languages.Select(l => new LanguageDto
            { 
                LanguageCode = l.LanguageCode, 
                LanguageName = l.DisplayName 
            }).ToList();
        
        return new GetSupportedLanguagesResponse()
        {
            SupportedLanguages = languages
        };
    }
}