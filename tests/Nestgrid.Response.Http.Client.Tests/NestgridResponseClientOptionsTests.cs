namespace Nestgrid.Response.Http.Client.Tests;

public sealed class NestgridResponseClientOptionsTests
{
    [Fact]
    public void Defaults_use_full_result_and_approved_mappings()
    {
        var options = new NestgridResponseClientOptions();

        options.PayloadMode.ShouldBe(NestgridResponsePayloadMode.FullResult);
        options.StatusMappings[200].ShouldBe(ResultStatus.Ok);
        options.StatusMappings[422].ShouldBe(ResultStatus.Failed);
        options.SerializerOptions.PropertyNameCaseInsensitive.ShouldBeTrue();
    }

    [Fact]
    public void Custom_mappings_are_copied_and_do_not_replace_unspecified_defaults()
    {
        var mappings = new Dictionary<int, ResultStatus> { [409] = ResultStatus.Cancelled };
        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.ValueOnly,
            statusMappings: mappings);

        mappings[409] = ResultStatus.Error;

        options.PayloadMode.ShouldBe(NestgridResponsePayloadMode.ValueOnly);
        options.StatusMappings[409].ShouldBe(ResultStatus.Cancelled);
        options.StatusMappings[404].ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public void Serializer_options_are_captured_as_a_copy()
    {
        var serializerOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            WriteIndented = true
        };

        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.FullResult,
            serializerOptions);

        serializerOptions.PropertyNameCaseInsensitive = true;
        serializerOptions.WriteIndented = false;

        options.SerializerOptions.PropertyNameCaseInsensitive.ShouldBeFalse();
        options.SerializerOptions.WriteIndented.ShouldBeTrue();
    }

    [Fact]
    public void Protocol_exception_exposes_only_safe_protocol_context()
    {
        var exception = new NestgridResponseProtocolException(
            "Invalid response envelope.",
            502,
            NestgridResponsePayloadMode.FullResult);

        exception.StatusCode.ShouldBe(502);
        exception.PayloadMode.ShouldBe(NestgridResponsePayloadMode.FullResult);
        exception.Message.ShouldNotContain("password");
    }
}
