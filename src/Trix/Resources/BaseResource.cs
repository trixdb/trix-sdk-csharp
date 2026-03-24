using System.Net.Http.Json;
using System.Text.Json;
using Trix.Internal;

namespace Trix.Resources;

/// <summary>
/// Base class for all API resources.
/// </summary>
public abstract class BaseResource
{
    /// <summary>
    /// The HTTP pipeline for making requests.
    /// </summary>
    private readonly HttpPipeline _pipeline;

    /// <summary>
    /// JSON serialization options.
    /// </summary>
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Initializes a new instance of the resource.
    /// </summary>
    /// <param name="pipeline">The HTTP pipeline.</param>
    internal BaseResource(HttpPipeline pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    /// <summary>
    /// Makes a GET request and returns the response.
    /// </summary>
    protected virtual async Task<T> GetAsync<T>(
        string path,
        Dictionary<string, string?>? queryParams = null,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            HttpMethod.Get,
            path,
            queryParams: queryParams,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes a POST request and returns the response.
    /// </summary>
    protected virtual async Task<T> PostAsync<T>(
        string path,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            HttpMethod.Post,
            path,
            body: body,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes a POST request with no response body.
    /// </summary>
    protected virtual async Task PostAsync(
        string path,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            HttpMethod.Post,
            path,
            body: body,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes a PUT request and returns the response.
    /// </summary>
    protected virtual async Task<T> PutAsync<T>(
        string path,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            HttpMethod.Put,
            path,
            body: body,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes a PATCH request and returns the response.
    /// </summary>
    protected virtual async Task<T> PatchAsync<T>(
        string path,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            new HttpMethod("PATCH"),
            path,
            body: body,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes a PATCH request with extra headers and returns the response.
    /// </summary>
    protected virtual async Task<T> PatchAsync<T>(
        string path,
        object? body,
        Dictionary<string, string>? headers,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            new HttpMethod("PATCH"),
            path,
            body: body,
            headers: headers,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes a DELETE request.
    /// </summary>
    protected virtual async Task DeleteAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            HttpMethod.Delete,
            path,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes a DELETE request and returns the response.
    /// </summary>
    protected virtual async Task<T> DeleteAsync<T>(
        string path,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendAsync(
            HttpMethod.Delete,
            path,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deserializes the response content.
    /// </summary>
    private static async Task<T> DeserializeAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return result ?? throw new InvalidOperationException("Response was null");
    }

    /// <summary>
    /// Makes a GET request and returns the response as a stream.
    /// The returned stream wraps the response and will dispose it when closed.
    /// </summary>
    protected virtual async Task<Stream> GetStreamAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        var response = await _pipeline.SendAsync(
            HttpMethod.Get,
            path,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        try
        {
            var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            // Return a wrapper that disposes the response when the stream is disposed
            return new ResponseOwningStream(contentStream, response);
        }
        catch
        {
            response.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Makes a POST request with multipart form data.
    /// </summary>
    protected virtual async Task<T> PostMultipartAsync<T>(
        string path,
        Stream fileData,
        string fileName,
        string contentType,
        Dictionary<string, object>? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        using var response = await _pipeline.SendMultipartAsync(
            path,
            fileData,
            fileName,
            contentType,
            additionalFields,
            cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// A stream wrapper that owns and disposes an HttpResponseMessage when the stream is disposed.
    /// </summary>
    private sealed class ResponseOwningStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly HttpResponseMessage _response;
        private int _disposed;

        public ResponseOwningStream(Stream innerStream, HttpResponseMessage response)
        {
            _innerStream = innerStream;
            _response = response;
        }

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length;
        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public override void Flush() => _innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
        public override void SetLength(long value) => _innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => _innerStream.ReadAsync(buffer, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                if (disposing)
                {
                    _innerStream.Dispose();
                    _response.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                try
                {
                    await _innerStream.DisposeAsync().ConfigureAwait(false);
                    _response.Dispose();
                }
                finally
                {
                    await base.DisposeAsync().ConfigureAwait(false);
                }
                return;
            }
            await base.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Builds a query string dictionary from optional parameters.
    /// </summary>
    protected static Dictionary<string, string?> BuildQueryParams(params (string key, object? value)[] parameters)
    {
        var result = new Dictionary<string, string?>();
        foreach (var (key, value) in parameters)
        {
            if (value != null)
            {
                result[key] = value.ToString();
            }
        }
        return result;
    }
}
