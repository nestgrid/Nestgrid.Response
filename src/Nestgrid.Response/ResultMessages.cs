namespace Nestgrid.Response;

/// <summary>
/// Provides factory methods for creating result messages.
/// </summary>
public static class ResultMessages
{
    /// <summary>
    /// Creates an informational result message.
    /// </summary>
    /// <param name="message">The human-readable message.</param>
    /// <param name="code">An optional machine-readable message code.</param>
    /// <param name="property">An optional related property.</param>
    /// <returns>A result message with informational severity.</returns>
    public static ResultMessage Info(
        string message,
        string? code = null,
        string? property = null) =>
        new(message, ResultMessageSeverity.Information, code, property);

    /// <summary>
    /// Creates a warning result message.
    /// </summary>
    /// <param name="message">The human-readable message.</param>
    /// <param name="code">An optional machine-readable message code.</param>
    /// <param name="property">An optional related property.</param>
    /// <returns>A result message with warning severity.</returns>
    public static ResultMessage Warning(
        string message,
        string? code = null,
        string? property = null) =>
        new(message, ResultMessageSeverity.Warning, code, property);

    /// <summary>
    /// Creates an error result message.
    /// </summary>
    /// <param name="message">The human-readable message.</param>
    /// <param name="code">An optional machine-readable message code.</param>
    /// <param name="property">An optional related property.</param>
    /// <returns>A result message with error severity.</returns>
    public static ResultMessage Error(
        string message,
        string? code = null,
        string? property = null) =>
        new(message, ResultMessageSeverity.Error, code, property);
}
