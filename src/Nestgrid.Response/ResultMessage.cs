namespace Nestgrid.Response;

/// <summary>
/// Represents structured result information.
/// </summary>
public sealed class ResultMessage
{
    internal ResultMessage(
        string message,
        ResultMessageSeverity severity,
        string? code = null,
        string? property = null)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        Message = message;
        Severity = severity;
        Code = code;
        Property = property;
    }

    /// <summary>
    /// Gets the human-readable message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets an optional machine-readable message code.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// Gets an optional related property.
    /// </summary>
    public string? Property { get; }

    /// <summary>
    /// Gets the severity of the message.
    /// </summary>
    public ResultMessageSeverity Severity { get; }
}
