namespace Nestgrid.Response.Http.Client.Internal;

internal sealed class NestgridResponseWireMessage
{
    public string? Message { get; set; }

    public string? Code { get; set; }

    public string? Property { get; set; }

    public ResultMessageSeverity Severity { get; set; }
}
