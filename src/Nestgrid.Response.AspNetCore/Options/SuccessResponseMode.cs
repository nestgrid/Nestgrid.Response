namespace Nestgrid.Response.AspNetCore.Options;

/// <summary>
/// Defines the payload shape for successful results.
/// </summary>
/// <remarks>
/// Results with <see cref="ResultStatus.NoContent"/> do not write a response body in any mode.
/// </remarks>
public enum SuccessResponseMode
{
    /// <summary>
    /// Writes the result object for successful generic results.
    /// </summary>
    FullResult,

    /// <summary>
    /// Writes only the value for successful generic results.
    /// </summary>
    ValueOnly
}
