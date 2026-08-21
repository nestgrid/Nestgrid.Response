namespace Nestgrid.Response.Http.Client;

/// <summary>Defines the response body representation expected by the client.</summary>
public enum NestgridResponsePayloadMode
{
    /// <summary>The response body is a Nestgrid result envelope.</summary>
    FullResult,
    /// <summary>A successful generic response contains only its value.</summary>
    ValueOnly
}
