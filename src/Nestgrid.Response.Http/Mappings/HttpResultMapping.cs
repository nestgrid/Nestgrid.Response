namespace Nestgrid.Response.Http.Mappings;

/// <summary>
/// Represents HTTP response metadata for a result.
/// </summary>
public readonly struct HttpResultMapping
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpResultMapping"/> struct without a body.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    public HttpResultMapping(int statusCode)
    {
        StatusCode = statusCode;
        HasBody = false;
        Body = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpResultMapping"/> struct with a body.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="body">The response body.</param>
    public HttpResultMapping(int statusCode, object? body)
    {
        StatusCode = statusCode;
        HasBody = true;
        Body = body;
    }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets a value indicating whether a response body should be written.
    /// </summary>
    public bool HasBody { get; }

    /// <summary>
    /// Gets the response body.
    /// </summary>
    public object? Body { get; }
}
