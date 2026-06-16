using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Nestgrid.Response;

/// <summary>
/// Represents a non-generic operation outcome.
/// </summary>
public class Result
{
    private static readonly IReadOnlyList<ResultMessage> EmptyMessages =
        Array.AsReadOnly(Array.Empty<ResultMessage>());

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="status">The semantic outcome of the operation.</param>
    /// <param name="messages">Messages to include.</param>
    protected internal Result(
        ResultStatus status,
        params ResultMessage[] messages)
    {
        Status = status;
        Messages = CopyMessages(messages);
    }

    /// <summary>
    /// Gets the semantic outcome of the operation.
    /// </summary>
    [JsonIgnore]
    public ResultStatus Status { get; }

    /// <summary>
    /// Gets the result messages.
    /// </summary>
    public IReadOnlyList<ResultMessage> Messages { get; }

    private static IReadOnlyList<ResultMessage> CopyMessages(ResultMessage[]? messages)
    {
        if (messages is null || messages.Length == 0)
        {
            return EmptyMessages;
        }

        var copy = new ResultMessage[messages.Length];
        Array.Copy(messages, copy, messages.Length);

        return new ReadOnlyCollection<ResultMessage>(copy);
    }
}
