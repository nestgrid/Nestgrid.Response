using System.Net;

namespace Nestgrid.Response.Http.Client.Tests;

public sealed class NestgridResponseHttpClientExtensionsTests
{
    [Fact]
    public async Task Non_generic_convenience_method_reads_a_result()
    {
        using var client = new HttpClient(new StaticHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Messages\":[]}", System.Text.Encoding.UTF8, "application/json")
            }));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/result");

        var result = await client.SendAndReadNestgridResponseAsync(request, new NestgridResponseReader());

        result.Status.ShouldBe(ResultStatus.Ok);
    }

    [Fact]
    public async Task Convenience_methods_reject_null_arguments()
    {
        using var client = new HttpClient(new StaticHandler(new HttpResponseMessage(HttpStatusCode.OK)));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/result");
        var reader = new NestgridResponseReader();

        var nullClient = await Should.ThrowAsync<ArgumentNullException>(
            () => NestgridResponseHttpClientExtensions.SendAndReadNestgridResponseAsync(
                null!, request, reader));
        nullClient.ParamName.ShouldBe("client");
        var nullRequest = await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync(null!, reader));
        nullRequest.ParamName.ShouldBe("request");
        var nullReader = await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync(request, null!));
        nullReader.ParamName.ShouldBe("reader");
        var nullGenericClient = await Should.ThrowAsync<ArgumentNullException>(
            () => NestgridResponseHttpClientExtensions.SendAndReadNestgridResponseAsync<Licence>(
                null!, request, reader));
        nullGenericClient.ParamName.ShouldBe("client");
        var nullGenericRequest = await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync<Licence>(null!, reader));
        nullGenericRequest.ParamName.ShouldBe("request");
        var nullGenericReader = await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync<Licence>(request, null!));
        nullGenericReader.ParamName.ShouldBe("reader");
    }

    [Fact]
    public async Task Null_request_or_reader_is_rejected_before_sending()
    {
        var handler = new CountingHandler();
        using var client = new HttpClient(handler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/result");

        await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync(null!, new NestgridResponseReader()));
        await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync(request, null!));
        await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync<Licence>(null!, new NestgridResponseReader()));
        await Should.ThrowAsync<ArgumentNullException>(
            () => client.SendAndReadNestgridResponseAsync<Licence>(request, null!));

        handler.CallCount.ShouldBe(0);
    }
    [Fact]
    public async Task Fake_handler_proves_standard_httpclient_composition()
    {
        using var client = new HttpClient(new StaticHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Id\":9,\"Name\":\"Finance\"}", System.Text.Encoding.UTF8, "application/json")
            }));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/licence");
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var result = await client.SendAndReadNestgridResponseAsync<Licence>(request, reader);

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
            () => client.SendAndReadNestgridResponseAsync(request, reader));
    }

    [Fact]
    public async Task Convenience_method_disposes_the_http_response_it_created()
    {
        var content = new StringContent("{\"Messages\":[]}", System.Text.Encoding.UTF8, "application/json");
        using var client = new HttpClient(new StaticHandler(
            new HttpResponseMessage(HttpStatusCode.OK) { Content = content }));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/result");

        await client.SendAndReadNestgridResponseAsync(request, new NestgridResponseReader());

        await Should.ThrowAsync<ObjectDisposedException>(() => content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Convenience_method_propagates_request_cancellation()
    {
        using var client = new HttpClient(new CancellingHandler());
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/result");
        using var cancellation = new CancellationTokenSource();

        var read = client.SendAndReadNestgridResponseAsync(
            request,
            new NestgridResponseReader(),
            cancellation.Token);
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() => read);
    }

    [Fact]
    public async Task Convenience_method_does_not_dispose_the_caller_owned_request()
    {
        using var client = new HttpClient(new StaticHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"Messages\":[]}", System.Text.Encoding.UTF8, "application/json")
        }));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/result");

        await client.SendAndReadNestgridResponseAsync(request, new NestgridResponseReader());

        request.Method.ShouldBe(HttpMethod.Get);
        request.RequestUri!.AbsoluteUri.ShouldBe("https://example.test/result");
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

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Messages\":[]}", System.Text.Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class CancellingHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
