namespace Nestgrid.Response.Extensions;

/// <summary>
/// Provides matching extensions for results.
/// </summary>
public static class ResultMatchExtensions
{
    /// <summary>
    /// Matches a generic result to a value by status.
    /// </summary>
    /// <remarks>
    /// Success statuses invoke <paramref name="success"/>. All other statuses invoke <paramref name="failure"/>.
    /// </remarks>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <typeparam name="TResult">The matched value type.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="success">The delegate to invoke for successful results.</param>
    /// <param name="failure">The delegate to invoke for non-success results.</param>
    /// <returns>The matched value.</returns>
    public static TResult Match<T, TResult>(
        this Result<T> result,
        Func<T?, TResult> success,
        Func<Result<T>, TResult> failure)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (success is null)
        {
            throw new ArgumentNullException(nameof(success));
        }

        if (failure is null)
        {
            throw new ArgumentNullException(nameof(failure));
        }

        return result.IsSuccess()
            ? success(result.Value)
            : failure(result);
    }

    /// <summary>
    /// Matches a non-generic result to a value by status.
    /// </summary>
    /// <remarks>
    /// Success statuses invoke <paramref name="success"/>. All other statuses invoke <paramref name="failure"/>.
    /// </remarks>
    /// <typeparam name="TResult">The matched value type.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="success">The delegate to invoke for successful results.</param>
    /// <param name="failure">The delegate to invoke for non-success results.</param>
    /// <returns>The matched value.</returns>
    public static TResult Match<TResult>(
        this Result result,
        Func<TResult> success,
        Func<Result, TResult> failure)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (success is null)
        {
            throw new ArgumentNullException(nameof(success));
        }

        if (failure is null)
        {
            throw new ArgumentNullException(nameof(failure));
        }

        return result.IsSuccess()
            ? success()
            : failure(result);
    }
}
