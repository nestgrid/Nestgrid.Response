namespace Nestgrid.Response;

/// <summary>
/// Represents the semantic outcome of an operation.
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Ok,

    /// <summary>
    /// The operation completed successfully and created a resource.
    /// </summary>
    Created,

    /// <summary>
    /// The operation was accepted for processing.
    /// </summary>
    Accepted,

    /// <summary>
    /// The operation completed successfully and has no content.
    /// </summary>
    NoContent,

    /// <summary>
    /// The operation failed because the supplied input was invalid.
    /// </summary>
    Invalid,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// The operation failed because the caller is not authenticated.
    /// </summary>
    Unauthorized,

    /// <summary>
    /// The operation failed because the caller is not allowed to perform it.
    /// </summary>
    Forbidden,

    /// <summary>
    /// The operation failed because of a conflict with the current state.
    /// </summary>
    Conflict,

    /// <summary>
    /// The operation was cancelled.
    /// </summary>
    Cancelled,

    /// <summary>
    /// The operation failed for an expected reason.
    /// </summary>
    Failed,

    /// <summary>
    /// The operation failed because of an unexpected error.
    /// </summary>
    Error
}
