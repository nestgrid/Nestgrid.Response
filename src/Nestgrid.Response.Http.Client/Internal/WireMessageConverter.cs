namespace Nestgrid.Response.Http.Client.Internal;

internal static class WireMessageConverter
{
    public static IReadOnlyList<ResultMessage> Convert(
        IReadOnlyList<NestgridResponseWireMessage>? wireMessages,
        int statusCode,
        NestgridResponsePayloadMode payloadMode)
    {
        if (wireMessages is null)
        {
            throw new NestgridResponseProtocolException(
                "The Nestgrid response envelope does not contain a messages array.",
                statusCode,
                payloadMode);
        }

        var messages = new List<ResultMessage>(wireMessages.Count);
        foreach (var wireMessage in wireMessages)
        {
            if (wireMessage is null || wireMessage.Message is null || !Enum.IsDefined(typeof(ResultMessageSeverity), wireMessage.Severity))
            {
                throw new NestgridResponseProtocolException(
                    "The Nestgrid response contains an invalid message.",
                    statusCode,
                    payloadMode);
            }

            messages.Add(wireMessage.Severity switch
            {
                ResultMessageSeverity.Information => ResultMessages.Info(wireMessage.Message, wireMessage.Code, wireMessage.Property),
                ResultMessageSeverity.Warning => ResultMessages.Warning(wireMessage.Message, wireMessage.Code, wireMessage.Property),
                ResultMessageSeverity.Error => ResultMessages.Error(wireMessage.Message, wireMessage.Code, wireMessage.Property),
                _ => throw new NestgridResponseProtocolException(
                    "The Nestgrid response contains an invalid message severity.",
                    statusCode,
                    payloadMode)
            });
        }

        return messages;
    }
}
