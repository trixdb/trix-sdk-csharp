using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix.Exceptions;
using Trix.Internal;

namespace Trix.Tests;

/// <summary>
/// Tests for HttpPipeline retry logic, error handling, and resource management.
/// </summary>
public class HttpPipelineTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly TrixClientOptions _options;

    public HttpPipelineTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.test.com")
        };
        _options = new TrixClientOptions
        {
            ApiKey = "test_key",
            BaseUrl = "https://api.test.com",
            MaxRetries = 3,
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    #region Retry Logic Tests

    [Fact]
    public async Task SendAsync_ServerError_RetriesWithExponentialBackoff()
    {
        // Arrange
        var attempts = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                attempts++;
                if (attempts < 3)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("{\"message\":\"Server error\"}")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\":\"test\"}")
                };
            });

        using var pipeline = CreatePipeline();

        // Act
        var response = await pipeline.SendAsync(HttpMethod.Get, "/test");

        // Assert
        attempts.Should().Be(3);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendAsync_NetworkError_RetriesAndThrowsNetworkException()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        using var pipeline = CreatePipeline();

        // Act & Assert
        var act = () => pipeline.SendAsync(HttpMethod.Get, "/test");
        await act.Should().ThrowAsync<NetworkException>()
            .WithMessage("*Network error*");

        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(4), // 1 + 3 retries
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_RateLimited_RespectsRetryAfterHeader()
    {
        // Arrange
        var attempts = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                attempts++;
                if (attempts == 1)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Content = new StringContent("{\"message\":\"Rate limited\"}")
                    };
                    response.Headers.Add("Retry-After", "1");
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\":\"test\"}")
                };
            });

        using var pipeline = CreatePipeline();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await pipeline.SendAsync(HttpMethod.Get, "/test");
        stopwatch.Stop();

        // Assert
        attempts.Should().Be(2);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(900); // ~1 second wait
    }

    [Fact]
    public async Task SendAsync_ClientError_DoesNotRetry()
    {
        // Arrange
        var attempts = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                attempts++;
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"message\":\"Bad request\"}")
                };
            });

        using var pipeline = CreatePipeline();

        // Act & Assert
        var act = () => pipeline.SendAsync(HttpMethod.Get, "/test");
        await act.Should().ThrowAsync<TrixException>();
        attempts.Should().Be(1); // No retries for 4xx
    }

    [Fact]
    public async Task SendAsync_Unauthorized_ThrowsAuthenticationException()
    {
        // Arrange
        SetupResponse(HttpStatusCode.Unauthorized, "{\"message\":\"Invalid token\",\"code\":\"AUTH001\"}",
            headers: new Dictionary<string, string> { { "X-Request-Id", "req-123" } });

        using var pipeline = CreatePipeline();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthenticationException>(
            () => pipeline.SendAsync(HttpMethod.Get, "/test"));

        ex.Message.Should().Be("Invalid token");
        ex.ErrorCode.Should().Be("AUTH001");
        ex.RequestId.Should().Be("req-123");
    }

    [Fact]
    public async Task SendAsync_Forbidden_ThrowsPermissionException()
    {
        // Arrange
        SetupResponse(HttpStatusCode.Forbidden, "{\"message\":\"Access denied\"}");

        using var pipeline = CreatePipeline();

        // Act & Assert
        await Assert.ThrowsAsync<PermissionException>(
            () => pipeline.SendAsync(HttpMethod.Get, "/test"));
    }

    [Fact]
    public async Task SendAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        SetupResponse(HttpStatusCode.NotFound, "{\"message\":\"Memory not found\"}");

        using var pipeline = CreatePipeline();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => pipeline.SendAsync(HttpMethod.Get, "/test"));
    }

    #endregion

    #region ValidationException Tests

    [Fact]
    public async Task SendAsync_ValidationError_ExtractsFieldErrors()
    {
        // Arrange
        var errorResponse = new
        {
            message = "Validation failed",
            code = "VALIDATION_ERROR",
            errors = new Dictionary<string, string[]>
            {
                { "content", new[] { "Content is required", "Content must be less than 10000 characters" } },
                { "tags", new[] { "Maximum 10 tags allowed" } }
            }
        };
        SetupResponse(HttpStatusCode.UnprocessableEntity, JsonSerializer.Serialize(errorResponse));

        using var pipeline = CreatePipeline();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => pipeline.SendAsync(HttpMethod.Post, "/test", new { content = "" }));

        ex.Message.Should().Be("Validation failed");
        ex.ErrorCode.Should().Be("VALIDATION_ERROR");
        ex.Errors.Should().NotBeNull();
        ex.Errors.Should().ContainKey("content");
        ex.Errors!["content"].Should().HaveCount(2);
        ex.Errors.Should().ContainKey("tags");
    }

    [Fact]
    public async Task SendAsync_ValidationErrorWithoutFieldErrors_HasNullErrors()
    {
        // Arrange
        SetupResponse(HttpStatusCode.UnprocessableEntity, "{\"message\":\"Invalid request\"}");

        using var pipeline = CreatePipeline();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => pipeline.SendAsync(HttpMethod.Post, "/test"));

        ex.Errors.Should().BeNull();
    }

    #endregion

    #region Rate Limit Tests

    [Fact]
    public async Task SendAsync_RateLimitWithHttpDate_ParsesCorrectly()
    {
        // Arrange
        var futureDate = DateTimeOffset.UtcNow.AddSeconds(2);
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent("{\"message\":\"Rate limited\"}")
        };
        response.Headers.Add("Retry-After", futureDate.ToString("R"));

        _mockHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

        using var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.SendAsync(HttpMethod.Get, "/test");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendAsync_RateLimitExhausted_ThrowsRateLimitException()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent("{\"message\":\"Rate limited\"}")
                };
                response.Headers.Add("Retry-After", "60");
                return response;
            });

        using var pipeline = CreatePipeline();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RateLimitException>(
            () => pipeline.SendAsync(HttpMethod.Get, "/test"));

        ex.RetryAfterSeconds.Should().Be(60);
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task SendAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage _, CancellationToken ct) =>
            {
                await Task.Delay(5000, ct);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        using var pipeline = CreatePipeline();

        // Act
        cts.CancelAfter(100);
        var act = () => pipeline.SendAsync(HttpMethod.Get, "/test", cancellationToken: cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SendAsync_Timeout_ThrowsTrixTimeoutException()
    {
        // Arrange
        var shortTimeoutOptions = new TrixClientOptions
        {
            ApiKey = "test_key",
            BaseUrl = "https://api.test.com",
            Timeout = TimeSpan.FromMilliseconds(100)
        };

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage _, CancellationToken ct) =>
            {
                await Task.Delay(5000, ct);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.test.com"),
            Timeout = TimeSpan.FromMilliseconds(100)
        };

        using var pipeline = new HttpPipeline(shortTimeoutOptions, httpClient);

        // Act & Assert
        var act = () => pipeline.SendAsync(HttpMethod.Get, "/test");
        await act.Should().ThrowAsync<TrixTimeoutException>();
    }

    #endregion

    #region Multipart Stream Tests

    [Fact]
    public async Task SendMultipartAsync_NonSeekableStream_ThrowsOnRetryNeeded()
    {
        // Arrange
        var attempts = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                attempts++;
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("{\"message\":\"Server error\"}")
                };
            });

        using var pipeline = CreatePipeline();
        using var nonSeekableStream = new NonSeekableStream(new byte[] { 1, 2, 3 });

        // Act & Assert
        var act = () => pipeline.SendMultipartAsync(
            "/upload",
            nonSeekableStream,
            "test.bin",
            "application/octet-stream");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*non-seekable*");
    }

    [Fact]
    public async Task SendMultipartAsync_SeekableStream_RetriesSuccessfully()
    {
        // Arrange
        var attempts = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                attempts++;
                if (attempts < 2)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("{\"message\":\"Server error\"}")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\":\"uploaded\"}")
                };
            });

        using var pipeline = CreatePipeline();
        using var seekableStream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act
        var response = await pipeline.SendMultipartAsync(
            "/upload",
            seekableStream,
            "test.bin",
            "application/octet-stream");

        // Assert
        attempts.Should().Be(2);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Response Disposal Tests

    [Fact]
    public async Task SendAsync_ErrorResponse_DisposesResponse()
    {
        // Arrange — a 500 with no retries so the pipeline throws immediately.
        var content = new DisposeTrackingContent("{\"message\":\"boom\"}");
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = content });

        _options.MaxRetries = 0;
        using var pipeline = CreatePipeline();

        // Act
        var act = () => pipeline.SendAsync(HttpMethod.Get, "/test");

        // Assert — the error response (and its pooled connection) must be released,
        // not leaked until finalization.
        await act.Should().ThrowAsync<ServerException>();
        content.Disposed.Should().BeTrue();
    }

    #endregion

    #region Helper Methods

    private HttpPipeline CreatePipeline()
    {
        return new HttpPipeline(_options, _httpClient);
    }

    private void SetupResponse(HttpStatusCode statusCode, string content,
        Dictionary<string, string>? headers = null)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                response.Headers.Add(key, value);
            }
        }

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// A stream wrapper that reports itself as non-seekable.
    /// </summary>
    private sealed class NonSeekableStream : Stream
    {
        private readonly MemoryStream _inner;

        public NonSeekableStream(byte[] data)
        {
            _inner = new MemoryStream(data);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false; // Key difference
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => _inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing) _inner.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// An HttpContent that records when it has been disposed, used to prove the
    /// pipeline disposes error responses instead of leaking them.
    /// </summary>
    private sealed class DisposeTrackingContent : HttpContent
    {
        private readonly byte[] _bytes;

        public bool Disposed { get; private set; }

        public DisposeTrackingContent(string json)
        {
            _bytes = Encoding.UTF8.GetBytes(json);
            Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            => stream.WriteAsync(_bytes, 0, _bytes.Length);

        protected override bool TryComputeLength(out long length)
        {
            length = _bytes.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    #endregion
}
