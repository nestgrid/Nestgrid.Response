namespace Nestgrid.Response;

/// <summary>
/// Provides factory methods for creating results.
/// </summary>
public static class Results
{
    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Ok"/> status.
    /// </summary>
    /// <returns>An ok result.</returns>
    public static Result Ok() => new(ResultStatus.Ok);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Ok"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An ok result.</returns>
    public static Result Ok(params ResultMessage[] messages) => new(ResultStatus.Ok, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Ok"/> status and a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">Value to include.</param>
    /// <returns>An ok result containing the value.</returns>
    public static Result<T> Ok<T>(T value) => new(ResultStatus.Ok, value);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Ok"/> status and a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">Value to include.</param>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An ok result containing the value.</returns>
    public static Result<T> Ok<T>(T value, params ResultMessage[] messages) => new(ResultStatus.Ok, value, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Created"/> status.
    /// </summary>
    /// <returns>A created result.</returns>
    public static Result Created() => new(ResultStatus.Created);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Created"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A created result.</returns>
    public static Result Created(params ResultMessage[] messages) => new(ResultStatus.Created, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Created"/> status and a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">Value to include.</param>
    /// <returns>A created result containing the value.</returns>
    public static Result<T> Created<T>(T value) => new(ResultStatus.Created, value);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Created"/> status and a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">Value to include.</param>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A created result containing the value.</returns>
    public static Result<T> Created<T>(T value, params ResultMessage[] messages) => new(ResultStatus.Created, value, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Accepted"/> status.
    /// </summary>
    /// <returns>An accepted result.</returns>
    public static Result Accepted() => new(ResultStatus.Accepted);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Accepted"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An accepted result.</returns>
    public static Result Accepted(params ResultMessage[] messages) => new(ResultStatus.Accepted, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Accepted"/> status and a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">Value to include.</param>
    /// <returns>An accepted result containing the value.</returns>
    public static Result<T> Accepted<T>(T value) => new(ResultStatus.Accepted, value);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Accepted"/> status and a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">Value to include.</param>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An accepted result containing the value.</returns>
    public static Result<T> Accepted<T>(T value, params ResultMessage[] messages) => new(ResultStatus.Accepted, value, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.NoContent"/> status.
    /// </summary>
    /// <returns>A no-content result.</returns>
    public static Result NoContent() => new(ResultStatus.NoContent);

    /// <summary>
    /// Creates a typed result with a <see cref="ResultStatus.NoContent"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A typed no-content result with the default value for <typeparamref name="T"/>.</returns>
    public static Result<T> NoContent<T>() => new(ResultStatus.NoContent);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Invalid"/> status.
    /// </summary>
    /// <returns>An invalid result.</returns>
    public static Result Invalid() => new(ResultStatus.Invalid);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Invalid"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>An invalid result.</returns>
    public static Result Invalid(string message) => Invalid(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Invalid"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An invalid result.</returns>
    public static Result Invalid(params ResultMessage[] messages) => new(ResultStatus.Invalid, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Invalid"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>An invalid result.</returns>
    public static Result<T> Invalid<T>() => new(ResultStatus.Invalid);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Invalid"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>An invalid result.</returns>
    public static Result<T> Invalid<T>(string message) => Invalid<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Invalid"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An invalid result.</returns>
    public static Result<T> Invalid<T>(params ResultMessage[] messages) => new(ResultStatus.Invalid, default, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.NotFound"/> status.
    /// </summary>
    /// <returns>A not-found result.</returns>
    public static Result NotFound() => new(ResultStatus.NotFound);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.NotFound"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>A not-found result.</returns>
    public static Result NotFound(string message) => NotFound(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.NotFound"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A not-found result.</returns>
    public static Result NotFound(params ResultMessage[] messages) => new(ResultStatus.NotFound, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.NotFound"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A not-found result.</returns>
    public static Result<T> NotFound<T>() => new(ResultStatus.NotFound);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.NotFound"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>A not-found result.</returns>
    public static Result<T> NotFound<T>(string message) => NotFound<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.NotFound"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A not-found result.</returns>
    public static Result<T> NotFound<T>(params ResultMessage[] messages) => new(ResultStatus.NotFound, default, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Unauthorized"/> status.
    /// </summary>
    /// <returns>An unauthorized result.</returns>
    public static Result Unauthorized() => new(ResultStatus.Unauthorized);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Unauthorized"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>An unauthorized result.</returns>
    public static Result Unauthorized(string message) => Unauthorized(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Unauthorized"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An unauthorized result.</returns>
    public static Result Unauthorized(params ResultMessage[] messages) => new(ResultStatus.Unauthorized, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Unauthorized"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>An unauthorized result.</returns>
    public static Result<T> Unauthorized<T>() => new(ResultStatus.Unauthorized);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Unauthorized"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>An unauthorized result.</returns>
    public static Result<T> Unauthorized<T>(string message) => Unauthorized<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Unauthorized"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An unauthorized result.</returns>
    public static Result<T> Unauthorized<T>(params ResultMessage[] messages) => new(ResultStatus.Unauthorized, default, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Forbidden"/> status.
    /// </summary>
    /// <returns>A forbidden result.</returns>
    public static Result Forbidden() => new(ResultStatus.Forbidden);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Forbidden"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>A forbidden result.</returns>
    public static Result Forbidden(string message) => Forbidden(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Forbidden"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A forbidden result.</returns>
    public static Result Forbidden(params ResultMessage[] messages) => new(ResultStatus.Forbidden, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Forbidden"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A forbidden result.</returns>
    public static Result<T> Forbidden<T>() => new(ResultStatus.Forbidden);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Forbidden"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>A forbidden result.</returns>
    public static Result<T> Forbidden<T>(string message) => Forbidden<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Forbidden"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A forbidden result.</returns>
    public static Result<T> Forbidden<T>(params ResultMessage[] messages) => new(ResultStatus.Forbidden, default, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Conflict"/> status.
    /// </summary>
    /// <returns>A conflict result.</returns>
    public static Result Conflict() => new(ResultStatus.Conflict);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Conflict"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>A conflict result.</returns>
    public static Result Conflict(string message) => Conflict(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Conflict"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A conflict result.</returns>
    public static Result Conflict(params ResultMessage[] messages) => new(ResultStatus.Conflict, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Conflict"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A conflict result.</returns>
    public static Result<T> Conflict<T>() => new(ResultStatus.Conflict);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Conflict"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>A conflict result.</returns>
    public static Result<T> Conflict<T>(string message) => Conflict<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Conflict"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A conflict result.</returns>
    public static Result<T> Conflict<T>(params ResultMessage[] messages) => new(ResultStatus.Conflict, default, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Cancelled"/> status.
    /// </summary>
    /// <returns>A cancelled result.</returns>
    public static Result Cancelled() => new(ResultStatus.Cancelled);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Cancelled"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>A cancelled result.</returns>
    public static Result Cancelled(string message) => Cancelled(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Cancelled"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A cancelled result.</returns>
    public static Result Cancelled(params ResultMessage[] messages) => new(ResultStatus.Cancelled, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Cancelled"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A cancelled result.</returns>
    public static Result<T> Cancelled<T>() => new(ResultStatus.Cancelled);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Cancelled"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>A cancelled result.</returns>
    public static Result<T> Cancelled<T>(string message) => Cancelled<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Cancelled"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A cancelled result.</returns>
    public static Result<T> Cancelled<T>(params ResultMessage[] messages) => new(ResultStatus.Cancelled, default, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Failed"/> status.
    /// </summary>
    /// <returns>A failed result.</returns>
    public static Result Failed() => new(ResultStatus.Failed);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Failed"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>A failed result.</returns>
    public static Result Failed(string message) => Failed(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Failed"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A failed result.</returns>
    public static Result Failed(params ResultMessage[] messages) => new(ResultStatus.Failed, messages);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Failed"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A failed result.</returns>
    public static Result<T> Failed<T>() => new(ResultStatus.Failed);

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Failed"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Failed<T>(string message) => Failed<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with a <see cref="ResultStatus.Failed"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Failed<T>(params ResultMessage[] messages) => new(ResultStatus.Failed, default, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status.
    /// </summary>
    /// <returns>An error result.</returns>
    public static Result Error() => new(ResultStatus.Error);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status.
    /// </summary>
    /// <param name="message">Error message to include.</param>
    /// <returns>An error result.</returns>
    public static Result Error(string message) => Error(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status.
    /// </summary>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An error result.</returns>
    public static Result Error(params ResultMessage[] messages) => new(ResultStatus.Error, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status from an exception.
    /// </summary>
    /// <param name="exception">The exception to convert into a result message.</param>
    /// <returns>An error result.</returns>
    /// <remarks>
    /// The exception is not stored on the result. The exception message is converted into a
    /// <see cref="ResultMessage"/>, and the exception type name is used as the message code.
    /// </remarks>
    public static Result Error(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        return Error(ResultMessages.Error(exception.Message, code: exception.GetType().Name));
    }

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>An error result.</returns>
    public static Result<T> Error<T>() => new(ResultStatus.Error);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">Error message to include.</param>
    /// <returns>An error result.</returns>
    public static Result<T> Error<T>(string message) => Error<T>(ResultMessages.Error(message));

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="messages">Messages to include.</param>
    /// <returns>An error result.</returns>
    public static Result<T> Error<T>(params ResultMessage[] messages) => new(ResultStatus.Error, default, messages);

    /// <summary>
    /// Creates a result with an <see cref="ResultStatus.Error"/> status from an exception.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="exception">The exception to convert into a result message.</param>
    /// <returns>An error result.</returns>
    /// <remarks>
    /// The exception is not stored on the result. The exception message is converted into a
    /// <see cref="ResultMessage"/>, and the exception type name is used as the message code.
    /// </remarks>
    public static Result<T> Error<T>(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        return Error<T>(ResultMessages.Error(exception.Message, code: exception.GetType().Name));
    }
}
