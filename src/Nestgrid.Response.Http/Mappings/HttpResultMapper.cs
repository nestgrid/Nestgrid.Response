using Nestgrid.Response.Extensions;
using Nestgrid.Response.Http.Options;

namespace Nestgrid.Response.Http.Mappings;

/// <summary>
/// Maps results to HTTP response metadata.
/// </summary>
public static class HttpResultMapper
{
    private static readonly NestgridResponseOptions DefaultOptions = new();

    /// <summary>
    /// Maps a result to HTTP response metadata.
    /// </summary>
    /// <param name="result">The result to map.</param>
    /// <param name="options">The response options.</param>
    /// <param name="hasValue">Whether a typed value is available.</param>
    /// <param name="value">The typed value.</param>
    /// <returns>The HTTP response mapping.</returns>
    public static HttpResultMapping Map(
        Result result,
        NestgridResponseOptions options,
        bool hasValue,
        object? value)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var statusCode = ResolveStatusCode(result, options);

        if (result.Status == ResultStatus.NoContent)
        {
            return new HttpResultMapping(statusCode);
        }

        if (hasValue &&
            result.IsSuccess() &&
            options.SuccessResponseMode == SuccessResponseMode.ValueOnly)
        {
            return new HttpResultMapping(statusCode, value);
        }

        return new HttpResultMapping(statusCode, result);
    }

    private static int ResolveStatusCode(Result result, NestgridResponseOptions options)
    {
        if (options.StatusMappings.TryGetValue(result.Status, out var statusCode))
        {
            return statusCode;
        }

        return DefaultOptions.StatusMappings[result.Status];
    }
}
