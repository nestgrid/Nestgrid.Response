using Nestgrid.Response.AspNetCore.Options;
using Nestgrid.Response.AspNetCore.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Nestgrid.Response.AspNetCore.Extensions;

/// <summary>
/// Provides ASP.NET Core result conversions.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a result to an <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The ASP.NET Core result.</returns>
    public static IResult ToIResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new ResponseAspNetCoreResult(result);
    }

    /// <summary>
    /// Converts a result to an <see cref="IResult"/>.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>The ASP.NET Core result.</returns>
    public static IResult ToIResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new ResponseAspNetCoreResult(result, result.Value);
    }

    /// <summary>
    /// Converts a result to an <see cref="IResult"/> using the specified options.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="options">The per-call options.</param>
    /// <returns>The ASP.NET Core result.</returns>
    public static IResult ToIResult(this Result result, NestgridResponseOptions options)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(options);

        return new ResponseAspNetCoreResult(result, options);
    }

    /// <summary>
    /// Converts a result to an <see cref="IResult"/> using the specified options.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="options">The per-call options.</param>
    /// <returns>The ASP.NET Core result.</returns>
    public static IResult ToIResult<T>(this Result<T> result, NestgridResponseOptions options)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(options);

        return new ResponseAspNetCoreResult(result, result.Value, options);
    }

    /// <summary>
    /// Converts a result to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The MVC action result.</returns>
    public static IActionResult ToActionResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new ResponseActionResult(result);
    }

    /// <summary>
    /// Converts a result to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>The MVC action result.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new ResponseActionResult(result, result.Value);
    }

    /// <summary>
    /// Converts a result to an <see cref="IActionResult"/> using the specified options.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="options">The per-call options.</param>
    /// <returns>The MVC action result.</returns>
    public static IActionResult ToActionResult(this Result result, NestgridResponseOptions options)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(options);

        return new ResponseActionResult(result, options);
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
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(options);

        return new ResponseActionResult(result, result.Value, options);
    }
}
