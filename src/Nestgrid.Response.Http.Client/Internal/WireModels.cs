using System.Text.Json;
using Nestgrid.Response;

namespace Nestgrid.Response.Http.Client.Internal;

internal sealed class NestgridResponseWireEnvelope
{
    public JsonElement Value { get; set; }

    public List<NestgridResponseWireMessage>? Messages { get; set; }
}

internal sealed class NestgridResponseWireMessage
{
    public string? Message { get; set; }

    public string? Code { get; set; }

    public string? Property { get; set; }

    public ResultMessageSeverity Severity { get; set; }
}
