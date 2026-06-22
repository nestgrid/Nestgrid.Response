namespace Nestgrid.Response.Extensions;

/// <summary>
/// Provides status classification extensions for results.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Determines whether the result has a success status.
    /// </summary>
    /// <remarks>
    /// Success statuses are <see cref="ResultStatus.Ok"/>, <see cref="ResultStatus.Created"/>,
    /// <see cref="ResultStatus.Accepted"/>, and <see cref="ResultStatus.NoContent"/>.
    /// </remarks>
    /// <param name="result">The result to inspect.</param>
    /// <returns><see langword="true"/> when the result status is successful.</returns>
    public static bool IsSuccess(this Result result)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        return result.Status is
            ResultStatus.Ok or
            ResultStatus.Created or
            ResultStatus.Accepted or
            ResultStatus.NoContent;
    }

    /// <summary>
    /// Determines whether the result has a non-success status.
    /// </summary>
    /// <remarks>
    /// Any status other than <see cref="ResultStatus.Ok"/>, <see cref="ResultStatus.Created"/>,
    /// <see cref="ResultStatus.Accepted"/>, or <see cref="ResultStatus.NoContent"/> is a non-success status.
    /// </remarks>
    /// <param name="result">The result to inspect.</param>
    /// <returns><see langword="true"/> when the result status is not successful.</returns>
    public static bool IsFailure(this Result result)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        return !result.IsSuccess();
    }
}
