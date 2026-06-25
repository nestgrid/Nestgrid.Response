using Nestgrid.Response.Http;
using Nestgrid.Response.Mvc.Internal;
using Microsoft.AspNetCore.Mvc;
using Nestgrid.Response.Http.Options;

namespace Nestgrid.Response.Mvc.Extensions;

/// <summary>
/// Provides MVC action result conversions.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a result to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The MVC action result.</returns>
    public static IActionResult ToActionResult(this Result result)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        return new ResponseMvcActionResult(result);
    }

    /// <summary>
    /// Converts a result to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>The MVC action result.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        return new ResponseMvcActionResult(result, result.Value);
    }

    /// <summary>
    /// Converts a result to an <see cref="IActionResult"/> using the specified options.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="options">The per-call options.</param>
    /// <returns>The MVC action result.</returns>
    public static IActionResult ToActionResult(this Result result, NestgridResponseOptions options)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return new ResponseMvcActionResult(result, options);
    }

    /// <summary>
    /// Converts a result to an <see cref="IActionResult"/> using the specified options.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="options">The per-call options.</param>
    /// <returns>The MVC action result.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result, NestgridResponseOptions options)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return new ResponseMvcActionResult(result, result.Value, options);
    }
}
