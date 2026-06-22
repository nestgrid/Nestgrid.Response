using Nestgrid.Response.AspNetCore.Options;
using Nestgrid.Response.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nestgrid.Response.AspNetCore.Internal;

internal static class HttpResultMapper
{
    private static NestgridResponseOptions DefaultOptions { get; } = new();

    internal static NestgridResponseOptions ResolveOptions(
        IServiceProvider requestServices,
        NestgridResponseOptions? perCallOptions)
    {
        if (perCallOptions is not null)
        {
            return perCallOptions;
        }

        return requestServices.GetService<IOptions<NestgridResponseOptions>>()?.Value
            ?? DefaultOptions;
    }

    internal static HttpResultMapping Map(
        Result result,
        NestgridResponseOptions options,
        bool hasValue,
        object? value)
    {
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
