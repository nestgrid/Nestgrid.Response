using System.Net.Http;

internal sealed class StaticHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage response;

    public StaticHandler(HttpResponseMessage response) => this.response = response;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) => Task.FromResult(response);
}
