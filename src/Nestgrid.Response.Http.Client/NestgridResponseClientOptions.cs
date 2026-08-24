using System.Collections.ObjectModel;
using System.Text.Json;

namespace Nestgrid.Response.Http.Client;

/// <summary>Configures HTTP response interpretation for the client adapter.</summary>
public sealed class NestgridResponseClientOptions
{
    private static readonly IReadOnlyDictionary<int, ResultStatus> DefaultStatusMappings =
        new ReadOnlyDictionary<int, ResultStatus>(new Dictionary<int, ResultStatus>
        {
            [200] = ResultStatus.Ok,
            [201] = ResultStatus.Created,
            [202] = ResultStatus.Accepted,
            [204] = ResultStatus.NoContent,
            [400] = ResultStatus.Invalid,
            [401] = ResultStatus.Unauthorized,
            [403] = ResultStatus.Forbidden,
            [404] = ResultStatus.NotFound,
            [409] = ResultStatus.Conflict,
            [422] = ResultStatus.Failed
        });

    /// <summary>Creates client options with the approved default policy.</summary>
    public NestgridResponseClientOptions() : this(NestgridResponsePayloadMode.FullResult) { }

    /// <summary>Creates client options.</summary>
    public NestgridResponseClientOptions(
        NestgridResponsePayloadMode payloadMode,
        JsonSerializerOptions? serializerOptions = null,
        IReadOnlyDictionary<int, ResultStatus>? statusMappings = null)
    {
        PayloadMode = payloadMode;
        SerializerOptions = CreateSerializerOptions(serializerOptions);

        var mappings = new Dictionary<int, ResultStatus>();
        foreach (var mapping in DefaultStatusMappings)
        {
            mappings[mapping.Key] = mapping.Value;
        }
        if (statusMappings is not null)
        {
            foreach (var mapping in statusMappings)
            {
                mappings[mapping.Key] = mapping.Value;
            }
        }

        StatusMappings = new ReadOnlyDictionary<int, ResultStatus>(mappings);
    }

    private static JsonSerializerOptions CreateSerializerOptions(JsonSerializerOptions? source)
    {
        if (source is null)
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        return new JsonSerializerOptions
        {
            AllowTrailingCommas = source.AllowTrailingCommas,
            DictionaryKeyPolicy = source.DictionaryKeyPolicy,
            Encoder = source.Encoder,
            IgnoreNullValues = source.IgnoreNullValues,
            MaxDepth = source.MaxDepth,
            PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive,
            PropertyNamingPolicy = source.PropertyNamingPolicy,
            ReadCommentHandling = source.ReadCommentHandling,
            WriteIndented = source.WriteIndented
        };
    }

    /// <summary>Gets the declared payload representation.</summary>
    public NestgridResponsePayloadMode PayloadMode { get; }

    /// <summary>Gets the immutable copy of serializer options.</summary>
    public JsonSerializerOptions SerializerOptions { get; }

    /// <summary>Gets the immutable client-owned status mappings.</summary>
    public IReadOnlyDictionary<int, ResultStatus> StatusMappings { get; }
}
