using System.Net;
using System.Text;

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
}
