using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nestgrid.Response.Http.Client.Tests;

public sealed class NestgridResponseReaderTests
{
    [Fact]
    public async Task Full_result_generic_success_preserves_value_and_messages()
    {
        using var response = Response(HttpStatusCode.OK, """
            {"Value":{"Id":7,"Name":"Licence"},"Messages":[{"Message":"Loaded","Code":"loaded","Property":"Name","Severity":0}]}
            """);
        var reader = new NestgridResponseReader();

        var result = await reader.ReadAsync<Licence>(response);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldBe(new Licence(7, "Licence"));
        result.Messages.Count.ShouldBe(1);
        result.Messages[0].Message.ShouldBe("Loaded");
        result.Messages[0].Code.ShouldBe("loaded");
        result.Messages[0].Property.ShouldBe("Name");
        result.Messages[0].Severity.ShouldBe(ResultMessageSeverity.Information);
    }

    [Fact]
    public async Task Full_result_failure_preserves_structured_messages()
    {
        using var response = Response(HttpStatusCode.UnprocessableEntity, """
            {"Messages":[{"Message":"Invalid","Code":"invalid","Property":"Name","Severity":2}]}
            """);

        var result = await new NestgridResponseReader().ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Failed);
        result.Messages.Single().Severity.ShouldBe(ResultMessageSeverity.Error);
        result.Messages.Single().Code.ShouldBe("invalid");
    }

    [Fact]
    public async Task Value_only_success_deserializes_only_the_declared_value()
    {
        using var response = Response(HttpStatusCode.Created, """{"Id":8,"Name":"Portal"}""");
        var reader = new NestgridResponseReader(new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var result = await reader.ReadAsync<Licence>(response);

        result.Status.ShouldBe(ResultStatus.Created);
        result.Value.ShouldBe(new Licence(8, "Portal"));
        result.Messages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Value_only_failure_reads_the_result_envelope()
    {
        using var response = Response(HttpStatusCode.BadRequest, """
            {"Messages":[{"Message":"Bad request","Code":"bad_request","Property":null,"Severity":2}]}
            """);
        var reader = new NestgridResponseReader(new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var result = await reader.ReadAsync<Licence>(response);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
        result.Messages.Single().Message.ShouldBe("Bad request");
    }

    [Fact]
    public async Task Value_only_non_generic_success_reads_the_explicit_result_envelope()
    {
        using var response = Response(HttpStatusCode.OK, """{"Messages":[{"Message":"Accepted","Severity":0}]}""");
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var result = await reader.ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Messages.Single().Message.ShouldBe("Accepted");
    }

    [Fact]
    public async Task No_content_is_bodyless_for_typed_and_non_generic_reads()
    {
        using var response = Response(HttpStatusCode.NoContent, "ignored");
        var reader = new NestgridResponseReader();

        var result = await reader.ReadAsync(response);
        var typed = await reader.ReadAsync<Licence>(response);

        result.Status.ShouldBe(ResultStatus.NoContent);
        typed.Status.ShouldBe(ResultStatus.NoContent);
        typed.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.Accepted)]
    public async Task Empty_non_generic_success_is_accepted(HttpStatusCode statusCode)
    {
        using var response = Response(statusCode, string.Empty);

        var result = await new NestgridResponseReader().ReadAsync(response);

        result.Status.ShouldBe(statusCode switch
        {
            HttpStatusCode.OK => ResultStatus.Ok,
            HttpStatusCode.Created => ResultStatus.Created,
            _ => ResultStatus.Accepted
        });
    }

    [Fact]
    public async Task Empty_generic_success_is_a_protocol_failure()
    {
        using var response = Response(HttpStatusCode.OK, string.Empty);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync<Licence>(response));

        exception.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task Custom_client_mapping_is_independent_from_server_mapping()
    {
        using var response = Response(HttpStatusCode.Conflict, """{"Messages":[]}""");
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            statusMappings: new Dictionary<int, ResultStatus> { [409] = ResultStatus.Cancelled });

        var result = await new NestgridResponseReader(options).ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Cancelled);
    }

    [Theory]
    [InlineData(200, ResultStatus.Ok)]
    [InlineData(201, ResultStatus.Created)]
    [InlineData(202, ResultStatus.Accepted)]
    [InlineData(400, ResultStatus.Invalid)]
    [InlineData(401, ResultStatus.Unauthorized)]
    [InlineData(403, ResultStatus.Forbidden)]
    [InlineData(404, ResultStatus.NotFound)]
    [InlineData(409, ResultStatus.Conflict)]
    [InlineData(422, ResultStatus.Failed)]
    [InlineData(500, ResultStatus.Error)]
    [InlineData(599, ResultStatus.Error)]
    public async Task Default_status_matrix_maps_observed_http_outcomes(int statusCode, ResultStatus expected)
    {
        using var response = Response((HttpStatusCode)statusCode, "{\"Messages\":[]}");

        var result = await new NestgridResponseReader().ReadAsync(response);

        result.Status.ShouldBe(expected);
    }

    [Fact]
    public async Task Three_hundred_and_fourteen_is_unmapped_by_default()
    {
        using var response = Response((HttpStatusCode)314, """{"Messages":[]}""");

        await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));
    }

    [Fact]
    public async Task Malformed_json_does_not_include_the_raw_body()
    {
        const string secretBody = "not-json-password=secret";
        using var response = Response(HttpStatusCode.OK, secretBody);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldNotContain(secretBody);
        exception.Message.ShouldNotContain("secret");
    }

    [Theory]
    [InlineData("{\"Value\":null}")]
    [InlineData("{\"Messages\":null}")]
    [InlineData("{\"Messages\":[null]}")]
    [InlineData("{\"Messages\":[{\"Message\":\"Bad\",\"Severity\":99}]}")]
    public async Task Invalid_message_collections_and_severity_are_protocol_failures(string body)
    {
        using var response = Response(HttpStatusCode.OK, body);

        await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));
    }

    [Fact]
    public async Task Non_json_media_types_are_rejected()
    {
        using var response = Response(HttpStatusCode.OK, "{\"Messages\":[]}");
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldContain("JSON media type");
    }

    [Fact]
    public async Task Missing_media_type_is_accepted_for_legacy_http_responses()
    {
        using var response = Response(HttpStatusCode.OK, "{\"Messages\":[]}");
        response.Content.Headers.ContentType = null;

        var result = await new NestgridResponseReader().ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Ok);
    }

    [Fact]
    public async Task Reader_isolated_from_later_serializer_option_mutation_and_preserves_converter()
    {
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(new LicenceConverter());
        var clientOptions = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.ValueOnly,
            serializerOptions);
        var reader = new NestgridResponseReader(clientOptions);
        clientOptions.SerializerOptions.Converters.Clear();

        using var response = Response(HttpStatusCode.OK, "\"custom\"");
        var result = await reader.ReadAsync<Licence>(response);

        result.Value.ShouldBe(new Licence(99, "custom"));
    }

    [Fact]
    public async Task Cancellation_during_body_read_is_propagated()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new DelayedContent()
        };
        using var cancellation = new CancellationTokenSource();
        var read = new NestgridResponseReader().ReadAsync(response, cancellation.Token);
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() => read);
    }

    [Fact]
    public async Task Reader_does_not_dispose_the_caller_owned_response()
    {
        using var response = Response(HttpStatusCode.OK, """{"Messages":[]}""");
        var reader = new NestgridResponseReader();

        await reader.ReadAsync(response);

        (await response.Content.ReadAsStringAsync()).ShouldBe("{\"Messages\":[]}");
    }

    private static HttpResponseMessage Response(HttpStatusCode statusCode, string body)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
    }

    private sealed record Licence(int Id, string Name);

    private sealed class LicenceConverter : JsonConverter<Licence>
    {
        public override Licence Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => new(99, reader.GetString()!);

        public override void Write(
            Utf8JsonWriter writer,
            Licence value,
            JsonSerializerOptions options) => writer.WriteStringValue(value.Name);
    }

    private sealed class DelayedContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            Task.CompletedTask;

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override Task<Stream> CreateContentReadStreamAsync() =>
            Task.FromResult<Stream>(new DelayedReadStream());
    }

    private sealed class DelayedReadStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count) => 0;

        public override Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken) => ReadUntilCancelledAsync(cancellationToken);

        private static async Task<int> ReadUntilCancelledAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return 0;
        }

        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
