namespace Nestgrid.Response;

/// <summary>
/// Represents the severity of a result message.
/// </summary>
public enum ResultMessageSeverity
{
    /// <summary>
    /// The message provides informational context.
    /// </summary>
    Information,

    /// <summary>
    /// The message describes a warning condition.
    /// </summary>
    Warning,

    /// <summary>
    /// The message describes an error condition.
    /// </summary>
    Error
}
