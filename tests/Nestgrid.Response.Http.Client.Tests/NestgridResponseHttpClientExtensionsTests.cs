using System.Net;
using System.Net.Http;

namespace Nestgrid.Response.Http.Client.Tests;

public sealed class NestgridResponseHttpClientExtensionsTests
{
    [Fact]
    public async Task Fake_handler_proves_standard_httpclient_composition()
    {
        using var client = new HttpClient(new StaticHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Id\":9,\"Name\":\"Finance\"}")
            }));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/licence");
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var result = await client.SendNestgridResponseAsync<Licence>(request, reader);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldBe(new Licence(9, "Finance"));
    }

    [Fact]
    public async Task Transport_failures_remain_standard_httpclient_exceptions()
    {
        using var client = new HttpClient(new ThrowingHandler());
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/failure");
        var reader = new NestgridResponseReader();

        await Should.ThrowAsync<HttpRequestException>(
            () => client.SendNestgridResponseAsync(request, reader));
    }

    private sealed record Licence(int Id, string Name);

    private sealed class StaticHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage response;

        public StaticHandler(HttpResponseMessage response) => this.response = response;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => Task.FromResult(response);
    }

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromException<HttpResponseMessage>(new HttpRequestException("transport failure"));
    }
}
