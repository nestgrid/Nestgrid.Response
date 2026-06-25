using Nestgrid.Response.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nestgrid.Response.Http.Mappings;
using Nestgrid.Response.Http.Options;

namespace Nestgrid.Response.Mvc.Internal;

internal sealed class ResponseMvcActionResult : IActionResult
{
    private readonly bool _hasValue;
    private readonly NestgridResponseOptions? _options;
    private readonly Result _result;
    private readonly object? _value;

    private static NestgridResponseOptions DefaultOptions { get; } = new();

    internal ResponseMvcActionResult(Result result)
    {
        _result = result;
    }

    internal ResponseMvcActionResult(Result result, NestgridResponseOptions options)
    {
        _result = result;
        _options = options;
    }

    internal ResponseMvcActionResult(Result result, object? value)
    {
        _result = result;
        _value = value;
        _hasValue = true;
    }

    internal ResponseMvcActionResult(Result result, object? value, NestgridResponseOptions options)
    {
        _result = result;
        _value = value;
        _options = options;
        _hasValue = true;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

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
