namespace Nestgrid.Response.AspNetCore.Internal;

internal readonly struct HttpResultMapping
{
    internal HttpResultMapping(int statusCode)
    {
        StatusCode = statusCode;
        HasBody = false;
        Body = null;
    }

    internal HttpResultMapping(int statusCode, object? body)
    {
        StatusCode = statusCode;
        HasBody = true;
        Body = body;
    }

    internal int StatusCode { get; }

    internal bool HasBody { get; }

    internal object? Body { get; }
}