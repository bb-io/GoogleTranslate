using Blackbird.Applications.Sdk.Common.Exceptions;
using Grpc.Core;

namespace Apps.GoogleTranslate.Utils;

public static class ErrorHandler
{
    public static async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        { 
            await action();
        }
        catch (RpcException ex)
        {
            throw new PluginApplicationException($"{ex.Message}");
        }
    }
    
    public static async Task<T> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (RpcException ex)
        {
            throw new PluginApplicationException($"{ex.Message}");
        }
    }
}