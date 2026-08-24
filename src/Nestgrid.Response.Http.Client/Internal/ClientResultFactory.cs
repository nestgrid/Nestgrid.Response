namespace Nestgrid.Response.Http.Client.Internal;

internal static class ClientResultFactory
{
    public static Result Create(ResultStatus status, IReadOnlyList<ResultMessage> messages)
    {
        var messageArray = messages.ToArray();
        return status switch
        {
            ResultStatus.Ok => Results.Ok(messageArray),
            ResultStatus.Created => Results.Created(messageArray),
            ResultStatus.Accepted => Results.Accepted(messageArray),
            ResultStatus.NoContent => Results.NoContent(),
            ResultStatus.Invalid => Results.Invalid(messageArray),
            ResultStatus.NotFound => Results.NotFound(messageArray),
            ResultStatus.Unauthorized => Results.Unauthorized(messageArray),
            ResultStatus.Forbidden => Results.Forbidden(messageArray),
            ResultStatus.Conflict => Results.Conflict(messageArray),
            ResultStatus.Cancelled => Results.Cancelled(messageArray),
            ResultStatus.Failed => Results.Failed(messageArray),
            ResultStatus.Error => Results.Error(messageArray),
            _ => throw new NestgridResponseProtocolException("The client status mapping is not supported.")
        };
    }

    public static Result<T> Create<T>(ResultStatus status, T value, IReadOnlyList<ResultMessage> messages)
    {
        var messageArray = messages.ToArray();
        return status switch
        {
            ResultStatus.Ok => Results.Ok(value, messageArray),
            ResultStatus.Created => Results.Created(value, messageArray),
            ResultStatus.Accepted => Results.Accepted(value, messageArray),
            ResultStatus.NoContent => Results.NoContent<T>(),
            ResultStatus.Invalid => Results.Invalid<T>(messageArray),
            ResultStatus.NotFound => Results.NotFound<T>(messageArray),
            ResultStatus.Unauthorized => Results.Unauthorized<T>(messageArray),
            ResultStatus.Forbidden => Results.Forbidden<T>(messageArray),
            ResultStatus.Conflict => Results.Conflict<T>(messageArray),
            ResultStatus.Cancelled => Results.Cancelled<T>(messageArray),
            ResultStatus.Failed => Results.Failed<T>(messageArray),
            ResultStatus.Error => Results.Error<T>(messageArray),
            _ => throw new NestgridResponseProtocolException("The client status mapping is not supported.")
        };
    }
}
