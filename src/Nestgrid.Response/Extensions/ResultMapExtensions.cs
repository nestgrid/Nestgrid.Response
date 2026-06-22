namespace Nestgrid.Response.Extensions;

/// <summary>
/// Provides mapping extensions for results.
/// </summary>
public static class ResultMapExtensions
{
    /// <summary>
    /// Maps the value when the result has a success status.
    /// </summary>
    /// <remarks>
    /// Non-success results preserve status and messages, and do not invoke the mapper.
    /// </remarks>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <typeparam name="TResult">The mapped value type.</typeparam>
    /// <param name="result">The source result.</param>
    /// <param name="mapper">The value mapper.</param>
    /// <returns>A result with the mapped value, or the original status and messages.</returns>
    public static Result<TResult> Map<T, TResult>(
        this Result<T> result,
        Func<T?, TResult> mapper)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        return result.IsSuccess()
            ? new Result<TResult>(result.Status, mapper(result.Value), CopyMessages(result))
            : new Result<TResult>(result.Status, default, CopyMessages(result));
    }

    private static ResultMessage[] CopyMessages(Result result)
    {
        var messages = new ResultMessage[result.Messages.Count];

        for (var index = 0; index < result.Messages.Count; index++)
        {
            messages[index] = result.Messages[index];
        }

        return messages;
    }
}
