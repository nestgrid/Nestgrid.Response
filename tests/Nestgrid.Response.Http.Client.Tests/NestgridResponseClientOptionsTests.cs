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
        options.MaxResponseBodyBytes.ShouldBe(NestgridResponseClientOptions.DefaultMaxResponseBodyBytes);
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
    public void Custom_converters_are_preserved_in_the_serializer_snapshot()
    {
        var serializerOptions = new System.Text.Json.JsonSerializerOptions();
        serializerOptions.Converters.Add(new StringToIntConverter());

        var options = new NestgridResponseClientOptions(
            NestgridResponsePayloadMode.ValueOnly,
            serializerOptions);

        serializerOptions.Converters.Clear();

        options.SerializerOptions.Converters.Count.ShouldBe(1);

        options.SerializerOptions.Converters[0].ShouldBeOfType<StringToIntConverter>();
    }

    [Fact]
    public void Non_positive_response_body_limits_are_rejected()
    {
        var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
            new NestgridResponseClientOptions(
                NestgridResponsePayloadMode.FullResult,
                maxResponseBodyBytes: 0));

        exception.ParamName.ShouldBe("maxResponseBodyBytes");
        exception.Message.ShouldContain("must be positive");
    }

    private sealed class StringToIntConverter : System.Text.Json.Serialization.JsonConverter<int>
    {
        public override int Read(
            ref System.Text.Json.Utf8JsonReader reader,
            Type typeToConvert,
            System.Text.Json.JsonSerializerOptions options) => int.Parse(reader.GetString()!);

        public override void Write(
            System.Text.Json.Utf8JsonWriter writer,
            int value,
            System.Text.Json.JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
    }
}
