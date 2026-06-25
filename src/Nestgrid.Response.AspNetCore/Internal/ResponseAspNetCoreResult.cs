using Nestgrid.Response.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nestgrid.Response.Http.Mappings;
using Nestgrid.Response.Http.Options;

namespace Nestgrid.Response.AspNetCore.Internal;

internal sealed class ResponseAspNetCoreResult : IResult
{
    private readonly bool _hasValue;
    private readonly NestgridResponseOptions? _options;
    private readonly Result _result;
    private readonly object? _value;

    private static NestgridResponseOptions DefaultOptions { get; } = new();

    internal ResponseAspNetCoreResult(Result result)
    {
        _result = result;
    }

    internal ResponseAspNetCoreResult(Result result, NestgridResponseOptions options)
    {
        _result = result;
        _options = options;
    }

    internal ResponseAspNetCoreResult(Result result, object? value)
    {
        _result = result;
        _value = value;
        _hasValue = true;
    }

    internal ResponseAspNetCoreResult(Result result, object? value, NestgridResponseOptions options)
    {
        _result = result;
        _value = value;
        _options = options;
        _hasValue = true;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var effectiveOptions = ResolveOptions(httpContext.RequestServices, _options);
        var mapping = HttpResultMapper.Map(_result, effectiveOptions, _hasValue, _value);

        httpContext.Response.StatusCode = mapping.StatusCode;

        return mapping.HasBody
            ? httpContext.Response.WriteAsJsonAsync(mapping.Body)
            : Task.CompletedTask;
    }

    private static NestgridResponseOptions ResolveOptions(
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
}
