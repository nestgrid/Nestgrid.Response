using System.Text;
using System.Text.Json;
using Nestgrid.Response.Http.Client.Internal;

namespace Nestgrid.Response.Http.Client;

/// <summary>
/// Reads caller-owned HTTP responses into Nestgrid results.
/// </summary>
public sealed class NestgridResponseReader
{
    private readonly NestgridResponseClientOptions options;

    /// <summary>
    /// Creates a reader with the approved default client policy.
    /// </summary>
    public NestgridResponseReader()
        : this(new NestgridResponseClientOptions())
    {
    }

    /// <summary>
    /// Creates a reader with the specified client policy.
    /// </summary>
    /// <param name="options">The client response options.</param>
    public NestgridResponseReader(NestgridResponseClientOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        this.options = new NestgridResponseClientOptions(
            options.PayloadMode,
            options.SerializerOptions,
            options.StatusMappings);
    }

    /// <summary>
    /// Reads a non-generic caller-owned HTTP response.
    /// </summary>
    public async Task<Result> ReadAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var statusCode = (int)response.StatusCode;
        var status = ResolveStatus(statusCode);

        if (status == ResultStatus.NoContent)
        {
            return Results.NoContent();
        }

        var body = await ReadResponseBodyAsync(response, statusCode, cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(body))
        {
            if (statusCode is 200 or 201 or 202)
            {
                return ClientResultFactory.Create(status, Array.Empty<ResultMessage>());
            }

            throw Protocol("A non-empty response envelope was required.", statusCode);
        }

        var envelope = DeserializeEnvelope(body, statusCode);
        var messages = WireMessageConverter.Convert(envelope.Messages, statusCode, options.PayloadMode);
        return ClientResultFactory.Create(status, messages);
    }

    /// <summary>
    /// Reads a typed caller-owned HTTP response.
    /// </summary>
    public async Task<Result<T>> ReadAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var statusCode = (int)response.StatusCode;
        var status = ResolveStatus(statusCode);

        if (status == ResultStatus.NoContent)
        {
            return Results.NoContent<T>();
        }

        var body = await ReadResponseBodyAsync(response, statusCode, cancellationToken).ConfigureAwait(false);

        if (options.PayloadMode == NestgridResponsePayloadMode.ValueOnly && IsSuccess(status))
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                throw Protocol("A value was required for the generic response.", statusCode);
            }

            return ClientResultFactory.Create(
                status,
                DeserializeValue<T>(body, statusCode),
                Array.Empty<ResultMessage>());
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw Protocol("A non-empty response envelope was required.", statusCode);
        }

        var envelope = DeserializeEnvelope(body, statusCode);
        var messages = WireMessageConverter.Convert(envelope.Messages, statusCode, options.PayloadMode);

        if (options.PayloadMode == NestgridResponsePayloadMode.FullResult && IsSuccess(status) &&
            envelope.Value.ValueKind == JsonValueKind.Undefined)
        {
            throw Protocol("The generic result envelope does not contain a value.", statusCode);
        }

        T value = envelope.Value.ValueKind == JsonValueKind.Undefined || envelope.Value.ValueKind == JsonValueKind.Null
            ? default!
            : DeserializeValue<T>(envelope.Value.GetRawText(), statusCode);

        return ClientResultFactory.Create(status, value, messages);
    }

    private ResultStatus ResolveStatus(int statusCode) =>
        ClientStatusPolicy.Resolve(statusCode, options.StatusMappings, options.PayloadMode);

    private async Task<string> ReadResponseBodyAsync(
        HttpResponseMessage response,
        int statusCode,
        CancellationToken cancellationToken)
    {
        ValidateMediaType(response, statusCode);
        var body = await ReadBodyAsync(response, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return body;
    }

    private static bool IsSuccess(ResultStatus status) =>
        status is ResultStatus.Ok or ResultStatus.Created or ResultStatus.Accepted or ResultStatus.NoContent;

    private async Task<string> ReadBodyAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var bufferStream = new MemoryStream();
        var buffer = new byte[81920];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
        {
            await bufferStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
        }

        return Encoding.UTF8.GetString(bufferStream.ToArray());
    }

    private void ValidateMediaType(HttpResponseMessage response, int statusCode)
    {
        var mediaType = response.Content?.Headers.ContentType?.MediaType;
        if (mediaType is null || mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
            mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw Protocol("The response content type is not a supported JSON media type.", statusCode);
    }

    private NestgridResponseWireEnvelope DeserializeEnvelope(string body, int statusCode)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<NestgridResponseWireEnvelope>(body, options.SerializerOptions);
            if (envelope is null)
            {
                throw new JsonException();
            }

            return envelope;
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException)
        {
            throw Protocol("The response body is not a valid Nestgrid envelope.", statusCode, exception);
        }
    }

    private T DeserializeValue<T>(string body, int statusCode)
    {
        try
        {
            var value = JsonSerializer.Deserialize<T>(body, options.SerializerOptions);
            if (value is null && typeof(T).IsValueType)
            {
                throw new JsonException();
            }

            return value!;
        }
        catch (NestgridResponseProtocolException)
        {
            throw;
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException)
        {
            throw Protocol("The response body is not a valid value for the requested type.", statusCode, exception);
        }
    }

    private NestgridResponseProtocolException Protocol(
        string message,
        int statusCode,
        Exception? innerException = null) =>
        new(message, statusCode, options.PayloadMode, innerException);
}
