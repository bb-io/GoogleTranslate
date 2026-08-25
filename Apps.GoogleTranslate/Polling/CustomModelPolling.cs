using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Apps.GoogleTranslate.Polling.Models;
using Apps.GoogleTranslate.Utils;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Grpc.Core;

namespace Apps.GoogleTranslate.Polling;

[PollingEventList("Custom models")]
public class CustomModelPolling(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [PollingEvent(
        "On custom model training completed",
        "Triggered once when custom model training succeeds, fails, or is cancelled")]
    public async Task<PollingEventResponse<CustomModelTrainingMemory, CustomModelTrainingResponse>>
        OnCustomModelTrainingCompleted(
            PollingEventRequest<CustomModelTrainingMemory> request,
            [PollingEventParameter] CustomModelTrainingIdentifier identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier.OperationName))
            throw new PluginMisconfigurationException("Operation name is required.");

        if (request.Memory is null)
        {
            return new PollingEventResponse<CustomModelTrainingMemory, CustomModelTrainingResponse>
            {
                FlyBird = false,
                Memory = NewMemory(false)
            };
        }

        if (request.Memory.Triggered)
        {
            return new PollingEventResponse<CustomModelTrainingMemory, CustomModelTrainingResponse>
            {
                FlyBird = false,
                Memory = NewMemory(true)
            };
        }

        var operation = await ErrorHandler.ExecuteWithErrorHandlingAsync(async () =>
            await Client.TranslateClient.PollOnceCreateModelAsync(identifier.OperationName));

        if (!operation.IsCompleted)
        {
            return new PollingEventResponse<CustomModelTrainingMemory, CustomModelTrainingResponse>
            {
                FlyBird = false,
                Memory = NewMemory(false)
            };
        }

        var operationError = operation.RpcMessage.Error;
        var model = operation.GetResultOrNull();
        var result = operationError is not null || model is null
            ? CreateFailureResponse(identifier.OperationName, operationError, operation.Exception)
            : new CustomModelTrainingResponse
            {
                OperationName = identifier.OperationName,
                Status = "Succeeded",
                IsSuccessful = true,
                CustomModel = CustomResourceMapper.ToResponse(model)
            };

        return new PollingEventResponse<CustomModelTrainingMemory, CustomModelTrainingResponse>
        {
            FlyBird = true,
            Result = result,
            Memory = NewMemory(true)
        };
    }

    private static CustomModelTrainingMemory NewMemory(bool triggered) => new()
    {
        LastPollingTime = DateTime.UtcNow,
        Triggered = triggered
    };

    private static CustomModelTrainingResponse CreateFailureResponse(
        string operationName,
        Google.Rpc.Status? operationError,
        Google.LongRunning.OperationFailedException? exception)
    {
        var errorCode = operationError?.Code ?? exception?.Status.Code;
        return new CustomModelTrainingResponse
        {
            OperationName = operationName,
            Status = errorCode == (int)StatusCode.Cancelled ? "Cancelled" : "Failed",
            IsSuccessful = false,
            ErrorCode = errorCode,
            ErrorMessage = operationError?.Message
                           ?? exception?.Status.Message
                           ?? exception?.Message
                           ?? "Model training completed without returning a model."
        };
    }
}
