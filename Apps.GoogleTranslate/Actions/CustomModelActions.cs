using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Apps.GoogleTranslate.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.Actions;

[ActionList("Custom models")]
public class CustomModelActions(
    InvocationContext invocationContext,
    IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action(
        "Create dataset",
        Description = "Create a dataset for training a custom translation model and optionally import a TSV or TMX file from Google Cloud Storage")]
    public async Task<CustomDatasetResponse> CreateDataset(
        [ActionParameter] CreateCustomDatasetRequest input)
    {
        ValidateDatasetInput(input);

        var operation = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await Client.TranslateClient.CreateDatasetAsync(new CreateDatasetRequest
            {
                ParentAsLocationName = GetCustomModelLocation(),
                Dataset = new Dataset
                {
                    DisplayName = input.Name,
                    SourceLanguageCode = input.SourceLanguageCode,
                    TargetLanguageCode = input.TargetLanguageCode
                }
            }));

        var completedOperation = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await operation.PollUntilCompletedAsync());

        if (completedOperation.IsFaulted)
        {
            throw new PluginApplicationException(
                completedOperation.Exception?.Message ?? "Google Cloud could not create the dataset.");
        }

        var createdDataset = completedOperation.Result;
        var gcsInputSource = input.File is null
            ? input.GcsInputSource
            : await UploadTrainingFile(createdDataset.Name, input.File, input.GcsBucketName!);

        if (string.IsNullOrWhiteSpace(gcsInputSource))
            return CustomResourceMapper.ToResponse(createdDataset);

        await ImportDatasetData(createdDataset.Name, gcsInputSource);

        var refreshedDataset = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await Client.TranslateClient.GetDatasetAsync(createdDataset.Name));
        var response = CustomResourceMapper.ToResponse(refreshedDataset);
        response.DataImported = true;
        response.ImportedDataGcsUri = gcsInputSource;

        return response;
    }

    [Action("Start custom model training", Description = "Start training a custom translation model from a populated dataset")]
    public async Task<StartCustomModelTrainingResponse> StartCustomModelTraining(
        [ActionParameter] StartCustomModelTrainingRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.DatasetName))
            throw new PluginMisconfigurationException("Dataset is required.");

        if (string.IsNullOrWhiteSpace(input.ModelName))
            throw new PluginMisconfigurationException("Model name is required.");

        var operation = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await Client.TranslateClient.CreateModelAsync(new CreateModelRequest
            {
                ParentAsLocationName = GetCustomModelLocation(),
                Model = new Model
                {
                    DisplayName = input.ModelName,
                    Dataset = input.DatasetName
                }
            }));

        return new StartCustomModelTrainingResponse
        {
            OperationName = operation.Name,
            Status = operation.Metadata?.State.ToString() ?? "Running"
        };
    }

    private LocationName GetCustomModelLocation() =>
        new(Client.LocationName.ProjectId, "us-central1");

    private async Task<string> UploadTrainingFile(
        string datasetName,
        FileReference file,
        string bucketName)
    {
        var fileName = file.Name
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault() ?? "training-data.tsv";
        var objectName = $"blackbird/google-translate/datasets/{Guid.NewGuid():N}/{fileName}";

        try
        {
            using var stream = await fileManagementClient.DownloadAsync(file);
            await Client.StorageClient.UploadObjectAsync(
                bucketName,
                objectName,
                GetTrainingFileContentType(file),
                stream);

            return $"gs://{bucketName}/{objectName}";
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException(
                $"Dataset '{datasetName}' was created, but the input file could not be uploaded to Google Cloud Storage: {ex.Message}");
        }
    }

    private static string GetTrainingFileContentType(FileReference file) =>
        Path.GetExtension(file.Name).Equals(".tmx", StringComparison.OrdinalIgnoreCase)
            ? "application/x-tmx+xml"
            : "text/tab-separated-values";

    private async Task ImportDatasetData(string datasetName, string gcsInputSource)
    {
        try
        {
            var importOperation = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await Client.TranslateClient.ImportDataAsync(new ImportDataRequest
                {
                    Dataset = datasetName,
                    InputConfig = new DatasetInputConfig
                    {
                        InputFiles =
                        {
                            new DatasetInputConfig.Types.InputFile
                            {
                                GcsSource = new GcsInputSource
                                {
                                    InputUri = gcsInputSource
                                }
                            }
                        }
                    }
                }));

            var completedImport = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
                await importOperation.PollUntilCompletedAsync());

            if (completedImport.IsFaulted)
            {
                throw new PluginApplicationException(
                    completedImport.Exception?.Message ?? "Google Cloud could not import the dataset data.");
            }
        }
        catch (PluginApplicationException ex)
        {
            throw new PluginApplicationException(
                $"Dataset '{datasetName}' was created, but training data import failed: {ex.Message}");
        }
    }

    private static void ValidateDatasetInput(CreateCustomDatasetRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new PluginMisconfigurationException("Dataset name is required.");

        if (string.IsNullOrWhiteSpace(input.SourceLanguageCode))
            throw new PluginMisconfigurationException("Source language code is required.");

        if (string.IsNullOrWhiteSpace(input.TargetLanguageCode))
            throw new PluginMisconfigurationException("Target language code is required.");

        if (input.SourceLanguageCode.Equals(input.TargetLanguageCode, StringComparison.OrdinalIgnoreCase))
            throw new PluginMisconfigurationException("Source and target language codes must be different.");

        if (!string.IsNullOrWhiteSpace(input.GcsInputSource) &&
            !input.GcsInputSource.StartsWith("gs://", StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginMisconfigurationException("GCS input source URL must start with 'gs://'.");
        }

        if (input.File is not null && !string.IsNullOrWhiteSpace(input.GcsInputSource))
        {
            throw new PluginMisconfigurationException(
                "Use either Input file or GCS input source URL, not both.");
        }

        if (input.File is not null && string.IsNullOrWhiteSpace(input.GcsBucketName))
            throw new PluginMisconfigurationException("GCS bucket name is required when Input file is specified.");

        if (input.File is null && !string.IsNullOrWhiteSpace(input.GcsBucketName))
            throw new PluginMisconfigurationException("GCS bucket name can only be used with Input file.");

        if (input.File is not null)
        {
            var extension = Path.GetExtension(input.File.Name);
            if (!extension.Equals(".tsv", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".tmx", StringComparison.OrdinalIgnoreCase))
            {
                throw new PluginMisconfigurationException("Input file must be a .tsv or .tmx file.");
            }
        }
    }
}
