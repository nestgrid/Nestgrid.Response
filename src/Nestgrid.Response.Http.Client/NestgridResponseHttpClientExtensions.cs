namespace Nestgrid.Response.Http.Client;

/// <summary>
/// Provides thin HttpClient conveniences over <see cref="NestgridResponseReader"/>.
/// </summary>
public static class NestgridResponseHttpClientExtensions
{
    /// <summary>
    /// Sends a caller-created request and reads the response as a non-generic result.
    /// </summary>
    /// <remarks>The response created by HttpClient is disposed by this method.</remarks>
    public static async Task<Result> SendAndReadNestgridResponseAsync(
        this HttpClient client,
        HttpRequestMessage request,
        NestgridResponseReader reader,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a caller-created request and reads the response as a typed result.
    /// </summary>
    /// <remarks>The response created by HttpClient is disposed by this method.</remarks>
    public static async Task<Result<T>> SendAndReadNestgridResponseAsync<T>(
        this HttpClient client,
        HttpRequestMessage request,
        NestgridResponseReader reader,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }
}
