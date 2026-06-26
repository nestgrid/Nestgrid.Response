using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nestgrid.Response.Http.Mappings;
using Nestgrid.Response.Http.Options;

namespace Nestgrid.Response.AspNetCore.Internal;

internal sealed class ResponseActionResult : IActionResult
{
    private readonly bool _hasValue;
    private readonly NestgridResponseOptions? _options;
    private readonly Result _result;
    private readonly object? _value;

    private static NestgridResponseOptions DefaultOptions { get; } = new();

    internal ResponseActionResult(Result result)
    {
        _result = result;
    }

    internal ResponseActionResult(Result result, NestgridResponseOptions options)
    {
        _result = result;
        _options = options;
    }

    internal ResponseActionResult(Result result, object? value)
    {
        _result = result;
        _value = value;
        _hasValue = true;
    }

    internal ResponseActionResult(Result result, object? value, NestgridResponseOptions options)
    {
        _result = result;
        _value = value;
        _options = options;
        _hasValue = true;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var effectiveOptions = ResolveOptions(context.HttpContext.RequestServices, _options);
        var mapping = HttpResultMapper.Map(_result, effectiveOptions, _hasValue, _value);

        IActionResult actionResult = mapping.HasBody
            ? new ObjectResult(mapping.Body) { StatusCode = mapping.StatusCode }
            : new StatusCodeResult(mapping.StatusCode);

        return actionResult.ExecuteResultAsync(context);
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
