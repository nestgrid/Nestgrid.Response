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
    public async Task Full_result_generic_success_requires_a_value()
    {
        using var response = Response(HttpStatusCode.OK, "{\"Messages\":[]}");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync<Licence>(response));

        exception.Message.ShouldBe("The generic result envelope does not contain a value.");
    }

    [Fact]
    public async Task Value_only_generic_success_requires_a_value()
    {
        using var response = Response(HttpStatusCode.OK, string.Empty);
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => reader.ReadAsync<Licence>(response));

        exception.Message.ShouldBe("A value was required for the generic response.");
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

    [Fact]
    public async Task No_content_does_not_read_or_validate_response_content()
    {
        var content = new TrackingContent();
        using var response = new HttpResponseMessage(HttpStatusCode.NoContent) { Content = content };
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

        var reader = new NestgridResponseReader();

        (await reader.ReadAsync(response)).Status.ShouldBe(ResultStatus.NoContent);
        (await reader.ReadAsync<Licence>(response)).Status.ShouldBe(ResultStatus.NoContent);
        content.ReadCount.ShouldBe(0);
    }

    [Fact]
    public async Task Response_size_limit_accepts_an_exact_non_generic_success_body()
    {
        const string body = "{\"Messages\":[]}";
        using var response = Response(HttpStatusCode.OK, body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body));

        var result = await new NestgridResponseReader(options).ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Ok);
    }

    [Fact]
    public async Task Response_size_limit_rejects_an_oversized_non_generic_failure_body()
    {
        const string body = "{\"Messages\":[{\"Message\":\"failed\",\"Severity\":2}]}";
        using var response = Response(HttpStatusCode.BadRequest, body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body) - 1);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync(response));

        exception.Message.ShouldBe("The response body exceeds the configured maximum size.");
        exception.StatusCode.ShouldBe(400);
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public async Task Response_size_limit_accepts_an_exact_non_generic_failure_body()
    {
        const string body = "{\"Messages\":[{\"Message\":\"failed\",\"Severity\":2}]}";
        using var response = Response(HttpStatusCode.BadRequest, body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body));

        var result = await new NestgridResponseReader(options).ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Messages.Single().Message.ShouldBe("failed");
    }

    [Fact]
    public async Task Response_size_limit_accepts_an_exact_generic_success_body()
    {
        const string body = "123";
        using var response = Response(HttpStatusCode.OK, body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.ValueOnly,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body));

        var result = await new NestgridResponseReader(options).ReadAsync<int>(response);

        result.Value.ShouldBe(123);
    }

    [Fact]
    public async Task Response_size_limit_rejects_an_oversized_generic_failure_body()
    {
        const string body = "{\"Messages\":[{\"Message\":\"failed\",\"Severity\":2}]}";
        using var response = Response(HttpStatusCode.BadRequest, body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body) - 1);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync<Licence>(response));

        exception.Message.ShouldBe("The response body exceeds the configured maximum size.");
        exception.StatusCode.ShouldBe(400);
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public async Task Response_size_limit_rejects_an_oversized_non_generic_success_body()
    {
        const string body = "{\"Messages\":[]}";
        using var response = Response(HttpStatusCode.OK, body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body) - 1);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync(response));

        exception.Message.ShouldBe("The response body exceeds the configured maximum size.");
        exception.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task Response_size_limit_rejects_an_oversized_generic_success_body()
    {
        const string body = "123";
        using var response = Response(HttpStatusCode.OK, body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.ValueOnly,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body) - 1);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync<int>(response));

        exception.Message.ShouldBe("The response body exceeds the configured maximum size.");
        exception.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task Response_size_limit_is_enforced_across_multiple_reads()
    {
        const string body = "{\"Messages\":[]}";
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ChunkedContent(body, chunkSize: 5)
        };
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: Encoding.UTF8.GetByteCount(body) - 1);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync(response));

        exception.Message.ShouldBe("The response body exceeds the configured maximum size.");
        exception.InnerException.ShouldBeNull();
    }

    [Fact]
    public async Task Response_size_limit_counts_utf8_bytes_at_the_boundary()
    {
        const string body = "{\"Messages\":[{\"Message\":\"café\",\"Severity\":0}]}";
        var byteCount = Encoding.UTF8.GetByteCount(body);
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: byteCount - 1);
        using var response = Response(HttpStatusCode.OK, body);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync(response));

        exception.Message.ShouldBe("The response body exceeds the configured maximum size.");
    }

    [Fact]
    public async Task Mid_stream_read_failure_is_not_converted_to_a_protocol_result()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new FailingContent()
        };
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        await Should.ThrowAsync<IOException>(
            () => new NestgridResponseReader().ReadAsync(response));
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
        exception.Message.ShouldBe("A non-empty response envelope was required.");
    }

    [Fact]
    public async Task Empty_non_success_responses_are_protocol_failures()
    {
        using var nonGenericResponse = Response(HttpStatusCode.BadRequest, string.Empty);
        using var genericResponse = Response(HttpStatusCode.BadRequest, string.Empty);

        var nonGenericException = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(nonGenericResponse));
        var genericException = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync<Licence>(genericResponse));

        nonGenericException.Message.ShouldBe("A non-empty response envelope was required.");
        genericException.Message.ShouldBe("A non-empty response envelope was required.");
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

    [Theory]
    [InlineData(400, ResultStatus.Invalid)]
    [InlineData(401, ResultStatus.Unauthorized)]
    [InlineData(403, ResultStatus.Forbidden)]
    [InlineData(404, ResultStatus.NotFound)]
    [InlineData(409, ResultStatus.Conflict)]
    [InlineData(422, ResultStatus.Failed)]
    [InlineData(500, ResultStatus.Error)]
    public async Task Generic_failure_statuses_construct_the_expected_result_family(
        int statusCode,
        ResultStatus expected)
    {
        using var response = Response((HttpStatusCode)statusCode, "{\"Messages\":[]}");

        var result = await new NestgridResponseReader().ReadAsync<Licence>(response);

        result.Status.ShouldBe(expected);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Custom_cancelled_mapping_constructs_a_cancelled_result()
    {
        using var response = Response((HttpStatusCode)499, "{\"Messages\":[]}");
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            statusMappings: new Dictionary<int, ResultStatus> { [499] = ResultStatus.Cancelled });

        var result = await new NestgridResponseReader(options).ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Cancelled);
    }

    [Fact]
    public async Task Generic_custom_cancelled_mapping_constructs_a_cancelled_result()
    {
        using var response = Response((HttpStatusCode)499, "{\"Messages\":[]}");
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            statusMappings: new Dictionary<int, ResultStatus> { [499] = ResultStatus.Cancelled });

        var result = await new NestgridResponseReader(options).ReadAsync<Licence>(response);

        result.Status.ShouldBe(ResultStatus.Cancelled);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Generic_unsupported_custom_mapping_fails_safely()
    {
        using var response = Response((HttpStatusCode)498, "{\"Messages\":[]}");
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            statusMappings: new Dictionary<int, ResultStatus> { [498] = (ResultStatus)999 });

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync<Licence>(response));

        exception.Message.ShouldBe("The client status mapping is not supported.");
    }

    [Fact]
    public async Task Unsupported_custom_mapping_fails_safely()
    {
        using var response = Response((HttpStatusCode)498, "{\"Messages\":[]}");
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            statusMappings: new Dictionary<int, ResultStatus> { [498] = (ResultStatus)999 });

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader(options).ReadAsync(response));

        exception.Message.ShouldBe("The client status mapping is not supported.");
    }

    [Fact]
    public async Task Three_hundred_and_fourteen_is_unmapped_by_default()
    {
        using var response = Response((HttpStatusCode)314, """{"Messages":[]}""");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldBe("The HTTP status code is not mapped by the client policy.");
    }

    [Fact]
    public async Task Malformed_json_does_not_include_the_raw_body()
    {
        const string secretBody = "not-json-password=secret";
        using var response = Response(HttpStatusCode.OK, secretBody);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldBe("The response body is not a valid Nestgrid envelope.");
        exception.Message.ShouldNotContain(secretBody);
        exception.Message.ShouldNotContain("secret");
    }

    [Fact]
    public async Task Json_null_envelope_is_a_protocol_failure()
    {
        using var response = Response(HttpStatusCode.OK, "null");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldBe("The response body is not a valid Nestgrid envelope.");
    }

    [Fact]
    public async Task Invalid_value_body_is_a_protocol_failure()
    {
        using var response = Response(HttpStatusCode.OK, "not-json");
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => reader.ReadAsync<Licence>(response));

        exception.Message.ShouldBe("The response body is not a valid value for the requested type.");
    }

    [Fact]
    public async Task Null_value_for_a_value_type_is_a_protocol_failure()
    {
        using var response = Response(HttpStatusCode.OK, "null");
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => reader.ReadAsync<int>(response));

        exception.Message.ShouldBe("The response body is not a valid value for the requested type.");
    }

    [Fact]
    public async Task Null_value_for_a_nullable_value_type_is_rejected_by_the_value_contract()
    {
        using var response = Response(HttpStatusCode.OK, "null");
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => reader.ReadAsync<int?>(response));

        exception.Message.ShouldBe("The response body is not a valid value for the requested type.");
    }

    [Fact]
    public async Task Null_value_for_a_reference_type_is_preserved()
    {
        using var response = Response(HttpStatusCode.OK, "null");
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var result = await reader.ReadAsync<Licence>(response);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Hostile_value_converter_failure_is_normalised_without_diagnostics()
    {
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(new HostileConverter());
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly, serializerOptions));
        using var response = Response(HttpStatusCode.OK, "\"custom\"");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => reader.ReadAsync<Licence>(response));

        exception.Message.ShouldBe("The response body is not a valid value for the requested type.");
        exception.InnerException.ShouldBeNull();
        exception.Message.ShouldNotContain("secret");
    }

    [Fact]
    public async Task Response_without_content_is_treated_as_an_empty_body()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        var result = await new NestgridResponseReader().ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Ok);
    }

    [Fact]
    public async Task Generic_response_without_content_requires_a_value_in_value_only_mode()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var reader = new NestgridResponseReader(
            new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => reader.ReadAsync<Licence>(response));

        exception.Message.ShouldBe("A value was required for the generic response.");
    }

    [Fact]
    public async Task Default_empty_content_is_treated_as_an_empty_body()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        var result = await new NestgridResponseReader().ReadAsync(response);

        result.Status.ShouldBe(ResultStatus.Ok);
    }

    [Fact]
    public async Task Generic_full_result_with_null_content_requires_an_envelope()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync<Licence>(response));

        exception.Message.ShouldBe("A non-empty response envelope was required.");
    }

    [Fact]
    public async Task Custom_content_stream_is_fully_read()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new FixedStreamContent("{\"Messages\":[{\"Message\":\"from-stream\",\"Severity\":0}]}")
        };
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var result = await new NestgridResponseReader().ReadAsync(response);

        result.Messages.Single().Message.ShouldBe("from-stream");
    }

    [Fact]
    public async Task Null_options_and_responses_are_rejected()
    {
        var optionsException = Should.Throw<ArgumentNullException>(() => new NestgridResponseReader(null!));
        optionsException.ParamName.ShouldBe("options");
        var reader = new NestgridResponseReader();

        var nonGenericException = await Should.ThrowAsync<ArgumentNullException>(
            () => reader.ReadAsync((HttpResponseMessage)null!));
        nonGenericException.ParamName.ShouldBe("response");
        var genericException = await Should.ThrowAsync<ArgumentNullException>(
            () => reader.ReadAsync<Licence>(null!));
        genericException.ParamName.ShouldBe("response");
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
    public async Task Missing_messages_array_has_a_safe_protocol_message()
    {
        using var response = Response(HttpStatusCode.OK, "{\"Value\":null}");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldBe("The Nestgrid response envelope does not contain a messages array.");
    }

    [Fact]
    public async Task Invalid_message_has_a_safe_protocol_message()
    {
        using var response = Response(HttpStatusCode.OK, "{\"Messages\":[null]}");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldBe("The Nestgrid response contains an invalid message.");
    }

    [Fact]
    public async Task Invalid_message_severity_has_a_safe_protocol_message()
    {
        using var response = Response(HttpStatusCode.OK, "{\"Messages\":[{\"Message\":\"Bad\",\"Severity\":99}]}");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldBe("The Nestgrid response contains an invalid message.");
    }

    [Fact]
    public async Task Non_json_media_types_are_rejected()
    {
        using var response = Response(HttpStatusCode.OK, "{\"Messages\":[]}");
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

        var exception = await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        exception.Message.ShouldBe("The response content type is not a supported JSON media type.");
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
    public async Task Cancellation_after_body_read_is_propagated_before_result_construction()
    {
        using var cancellation = new CancellationTokenSource();
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new CancellingContent(cancellation)
        };
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        await Should.ThrowAsync<OperationCanceledException>(
            () => new NestgridResponseReader().ReadAsync(response, cancellation.Token));
    }

    [Fact]
    public async Task Cancellation_during_oversized_body_is_propagated()
    {
        using var cancellation = new CancellationTokenSource();
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new CancellingContent(cancellation)
        };
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            maxResponseBodyBytes: 1_048_576);

        await Should.ThrowAsync<OperationCanceledException>(
            () => new NestgridResponseReader(options).ReadAsync(response, cancellation.Token));
    }

    [Fact]
    public async Task Cancellation_before_non_generic_read_is_propagated()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => new NestgridResponseReader().ReadAsync(response, cancellation.Token));
    }

    [Fact]
    public async Task Cancellation_before_generic_read_is_propagated()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => new NestgridResponseReader().ReadAsync<int>(response, cancellation.Token));
    }

    [Fact]
    public async Task Reader_does_not_dispose_the_caller_owned_response()
    {
        using var response = Response(HttpStatusCode.OK, """{"Messages":[]}""");
        var reader = new NestgridResponseReader();

        await reader.ReadAsync(response);

        (await response.Content.ReadAsStringAsync()).ShouldBe("{\"Messages\":[]}");
    }

    [Fact]
    public async Task Reader_disposes_the_response_stream_after_a_read_failure()
    {
        var stream = new TrackingReadStream(Encoding.UTF8.GetBytes("not-json"));
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(stream)
        };
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        await Should.ThrowAsync<NestgridResponseProtocolException>(
            () => new NestgridResponseReader().ReadAsync(response));

        stream.WasDisposed.ShouldBeTrue();
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

    private sealed class HostileConverter : JsonConverter<Licence>
    {
        public override Licence Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            throw new InvalidOperationException("secret converter detail", new Exception("nested secret"));

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

    private sealed class FixedStreamContent : HttpContent
    {
        private readonly byte[] bytes;

        public FixedStreamContent(string body) => bytes = Encoding.UTF8.GetBytes(body);

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            stream.WriteAsync(bytes, 0, bytes.Length);

        protected override bool TryComputeLength(out long length)
        {
            length = bytes.Length;
            return true;
        }

        protected override Task<Stream> CreateContentReadStreamAsync() =>
            Task.FromResult<Stream>(new MemoryStream(bytes, writable: false));
    }

    private sealed class ChunkedContent : HttpContent
    {
        private readonly byte[] bytes;
        private readonly int chunkSize;

        public ChunkedContent(string body, int chunkSize)
        {
            bytes = Encoding.UTF8.GetBytes(body);
            this.chunkSize = chunkSize;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            stream.WriteAsync(bytes, 0, bytes.Length);

        protected override bool TryComputeLength(out long length)
        {
            length = bytes.Length;
            return true;
        }

        protected override Task<Stream> CreateContentReadStreamAsync() =>
            Task.FromResult<Stream>(new ChunkedReadStream(bytes, chunkSize));
    }

    private sealed class ChunkedReadStream : MemoryStream
    {
        private readonly int chunkSize;

        public ChunkedReadStream(byte[] bytes, int chunkSize)
            : base(bytes, writable: false) => this.chunkSize = chunkSize;

        public override Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken) =>
            base.ReadAsync(buffer, offset, Math.Min(count, chunkSize), cancellationToken);
    }

    private sealed class FailingContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            Task.CompletedTask;

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override Task<Stream> CreateContentReadStreamAsync() =>
            Task.FromResult<Stream>(new FailingReadStream());
    }

    private sealed class FailingReadStream : MemoryStream
    {
        public override Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken) =>
            Task.FromException<int>(new IOException("stream failure"));
    }

    private sealed class TrackingReadStream : MemoryStream
    {
        public TrackingReadStream(byte[] bytes) : base(bytes, writable: false) { }

        public bool WasDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class TrackingContent : HttpContent
    {
        public int ReadCount { get; private set; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            ReadCount++;
            return Task.CompletedTask;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return true;
        }

        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            ReadCount++;
            return Task.FromResult<Stream>(new MemoryStream());
        }
    }

    private sealed class CancellingContent : HttpContent
    {
        private readonly CancellationTokenSource cancellation;

        public CancellingContent(CancellationTokenSource cancellation) => this.cancellation = cancellation;

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            Task.CompletedTask;

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override Task<Stream> CreateContentReadStreamAsync() =>
            Task.FromResult<Stream>(new CancellingReadStream(cancellation));
    }

    private sealed class CancellingReadStream : MemoryStream
    {
        private readonly CancellationTokenSource cancellation;
        private bool returnedBody;

        public CancellingReadStream(CancellationTokenSource cancellation)
            : base(Encoding.UTF8.GetBytes("{\"Messages\":[]}")) => this.cancellation = cancellation;

        public override Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken)
        {
            if (!returnedBody)
            {
                returnedBody = true;
                return base.ReadAsync(buffer, offset, count, cancellationToken);
            }

            cancellation.Cancel();
            return Task.FromResult(0);
        }
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
