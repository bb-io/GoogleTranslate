﻿using Apps.GoogleTranslate.Extensions;
using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Apps.GoogleTranslate.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.Actions;

/* ActionList attribute was removed due to we want to keep this function but not expose it to the user */
public class AdaptiveDatasetActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Get all adaptive datasets", Description = "List adaptive datasets")]
    public async Task<GetAllAdaptiveMtResponse> GetAdaptiveDatasetsAsync()
    {
        string parent = Client.LocationName.ToString();
        var datasets = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => Client.TranslateClient.ListAdaptiveMtDatasets(new ListAdaptiveMtDatasetsRequest()
        {
            Parent = parent,
        }));

        return new GetAllAdaptiveMtResponse
        {
            AdaptiveMts = datasets.Select(x => new AdaptiveMtResponse
            {
                DisplayName = x.DisplayName,
                Name = x.Name,
                SourceLanguageCode = x.SourceLanguageCode,
                TargetLanguageCode = x.TargetLanguageCode,
                ExampleCount = x.ExampleCount
            }).ToList()
        };
    }

    [Action("Get adaptive dataset", Description = "Get adaptive machine translation dataset based on ID")]
    public async Task<AdaptiveMtResponse> GetAdaptiveDatasetAsync([ActionParameter] GetAdaptiveDatasetRequest request)
    {
        var dataset = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.TranslateClient.GetAdaptiveMtDatasetAsync(new GetAdaptiveMtDatasetRequest
        {
            Name = request.AdaptiveDatasetName
        }));

        return new AdaptiveMtResponse
        {
            DisplayName = dataset.DisplayName,
            Name = dataset.Name,
            SourceLanguageCode = dataset.SourceLanguageCode,
            TargetLanguageCode = dataset.TargetLanguageCode,
            ExampleCount = dataset.ExampleCount
        };
    }
    
    [Action("Create adaptive dataset", Description = "Create adaptive machine translation dataset")]
    public async Task<CreateAdaptiveMtResponse> CreateAdaptiveMtAsync([ActionParameter] CreateAdaptiveDatasetRequest request)
    {
        var parent = Client.LocationName.ToString().Replace("/global", "/us-central1");
        var createdDataset = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.TranslateClient.CreateAdaptiveMtDatasetAsync(
            new CreateAdaptiveMtDatasetRequest
            {
                Parent = parent,
                AdaptiveMtDataset = new AdaptiveMtDataset
                {
                    SourceLanguageCode = request.SourceLanguageCode,
                    TargetLanguageCode = request.TargetLanguageCode,
                    DisplayName = request.Name,
                    Name = $"{parent}/adaptiveMtDatasets/{request.Name}",
                    ExampleCount = request.ExampleCount ?? 10
                }
            }));

        var importRequest = new ImportAdaptiveMtFileRequest
        {
            Parent = parent,
            ParentAsAdaptiveMtDatasetName = new AdaptiveMtDatasetName(
                Client.ProjectName.ProjectId,
                Client.LocationName.LocationId.Replace("global", "us-central1"),
                createdDataset.AdaptiveMtDatasetName.DatasetId)
        };

        if (!string.IsNullOrEmpty(request.GcsInputSource))
        {
            importRequest.GcsInputSource = new GcsInputSource
            {
                InputUri = request.GcsInputSource
            };
        }
        
        if (request.File != null)
        {
            var stream = await fileManagementClient.DownloadAsync(request.File);
            var byteString = await stream.ToByteStringAsync();
            
            importRequest.FileInputSource = new FileInputSource
            {
                Content = byteString,
                MimeType = request.File?.ContentType,
                DisplayName = request.File?.Name
            };
        }
        
        var datasetResponse = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => 
            await Client.TranslateClient.ImportAdaptiveMtFileAsync(importRequest));

        return new CreateAdaptiveMtResponse
        {
            DisplayName = datasetResponse.AdaptiveMtFile.DisplayName,
            Name = datasetResponse.AdaptiveMtFile.Name,
            EntryCount = datasetResponse.AdaptiveMtFile.EntryCount
        };
    }
    
    [Action("Delete adaptive dataset", Description = "Delete adaptive machine translation dataset based on ID")]
    public async Task DeleteAdaptiveDatasetAsync([ActionParameter] GetAdaptiveDatasetRequest request)
    {
        await ErrorHandler.ExecuteWithErrorHandlingAsync(async () => await Client.TranslateClient.DeleteAdaptiveMtDatasetAsync(new DeleteAdaptiveMtDatasetRequest
        {
            Name = request.AdaptiveDatasetName
        }));
    }
}