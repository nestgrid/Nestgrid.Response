namespace Nestgrid.Response.Http.Client;

/// <summary>Represents a response that does not conform to the declared Nestgrid protocol.</summary>
public sealed class NestgridResponseProtocolException : Exception
{
    /// <summary>Creates a protocol exception.</summary>
    public NestgridResponseProtocolException(
        string message,
        int? statusCode = null,
        NestgridResponsePayloadMode? payloadMode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        PayloadMode = payloadMode;
    }

    /// <summary>Gets the observed HTTP status code, when available.</summary>
    public int? StatusCode { get; }

    /// <summary>Gets the declared payload mode, when available.</summary>
    public NestgridResponsePayloadMode? PayloadMode { get; }
}
