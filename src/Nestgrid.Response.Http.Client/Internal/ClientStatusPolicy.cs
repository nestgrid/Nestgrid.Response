namespace Nestgrid.Response.Http.Client.Internal;

using Nestgrid.Response;

internal static class ClientStatusPolicy
{
    public static ResultStatus Resolve(
        int statusCode,
        IReadOnlyDictionary<int, ResultStatus> mappings,
        NestgridResponsePayloadMode payloadMode)
    {
        if (mappings.TryGetValue(statusCode, out var mappedStatus))
        {
            return mappedStatus;
        }

        if (statusCode is >= 500 and <= 599)
        {
            return ResultStatus.Error;
        }

        throw new NestgridResponseProtocolException(
            "The HTTP status code is not mapped by the client policy.",
            statusCode,
            payloadMode);
    }
}
