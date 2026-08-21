using System.Text.Json;
using Nestgrid.Response;

namespace Nestgrid.Response.Http.Client.Internal;

internal sealed class NestgridResponseWireEnvelope
{
    public JsonElement Value { get; set; }

    public List<NestgridResponseWireMessage>? Messages { get; set; }
}
