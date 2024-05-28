using Google.Protobuf;

namespace Apps.GoogleTranslate.Extensions;

public static class StreamExtensions
{
    public static async Task<ByteString> ToByteStringAsync(this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return ByteString.CopyFrom(memoryStream.ToArray());
    }
}