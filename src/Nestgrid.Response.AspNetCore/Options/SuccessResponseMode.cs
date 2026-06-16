namespace Nestgrid.Response.AspNetCore.Options;

/// <summary>
/// Defines the payload shape for successful results.
/// </summary>
public enum SuccessResponseMode
{
    /// <summary>
    /// Writes the result object.
    /// </summary>
    FullResult,

    /// <summary>
    /// Writes only the value.
    /// </summary>
    ValueOnly
}
