using Nestgrid.Response.Http.Mappings;

namespace Nestgrid.Response.Http.Options;

/// <summary>
/// Configures result-to-response conversion.
/// </summary>
public sealed class NestgridResponseOptions
{
    /// <summary>
    /// Gets or sets the payload shape for successful results.
    /// </summary>
    public SuccessResponseMode SuccessResponseMode { get; set; } = SuccessResponseMode.FullResult;

    /// <summary>
    /// Gets the HTTP status code mappings.
    /// </summary>
    public IDictionary<ResultStatus, int> StatusMappings { get; } = DefaultStatusMappings.Create();
}
