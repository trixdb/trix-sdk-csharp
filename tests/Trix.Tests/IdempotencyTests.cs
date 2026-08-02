using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix.Internal;

namespace Trix.Tests;

/// <summary>
/// Tests that mutating requests carry a single stable Idempotency-Key across
/// retries (issue #2), and that non-mutating requests carry none.
/// </summary>
public class IdempotencyTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly TrixClientOptions _options;

    public IdempotencyTests()
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

    public void Dispose() => _httpClient.Dispose();

    private HttpPipeline CreatePipeline() => new(_options, _httpClient);

    /// <summary>
    /// Records the Idempotency-Key header of every attempt, then returns a 500
    /// on the first attempt and a 200 on the second (forces exactly one retry).
    /// </summary>
    private List<string?> CaptureKeysWith500Then200()
    {
        var keys = new List<string?>();
        var attempts = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
                keys.Add(req.Headers.TryGetValues("Idempotency-Key", out var v)
                    ? v.FirstOrDefault()
                    : null))
            .ReturnsAsync(() =>
            {
                attempts++;
                return attempts < 2
                    ? new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("{\"message\":\"Server error\"}")
                    }
                    : new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"id\":\"ok\"}")
                    };
            });
        return keys;
    }

    [Fact]
    public async Task SendAsync_PostRetry_ReusesSameIdempotencyKey()
    {
        var keys = CaptureKeysWith500Then200();
        using var pipeline = CreatePipeline();

        var response = await pipeline.SendAsync(HttpMethod.Post, "/things", new { name = "x" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        keys.Should().HaveCount(2);
        keys[0].Should().NotBeNullOrEmpty();
        Guid.TryParse(keys[0], out _).Should().BeTrue("the key should be a GUID");
        keys[1].Should().Be(keys[0], "the same key must be resent on every retry");
    }

    [Fact]
    public async Task SendAsync_GetRetry_SendsNoIdempotencyKey()
    {
        var keys = CaptureKeysWith500Then200();
        using var pipeline = CreatePipeline();

        var response = await pipeline.SendAsync(HttpMethod.Get, "/things");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        keys.Should().HaveCount(2);
        keys.Should().OnlyContain(k => k == null, "GET is not a mutating method");
    }

    [Theory]
    [InlineData("PUT")]
    [InlineData("PATCH")]
    [InlineData("DELETE")]
    public async Task SendAsync_MutatingRetry_ReusesSameIdempotencyKey(string method)
    {
        var keys = CaptureKeysWith500Then200();
        using var pipeline = CreatePipeline();

        var response = await pipeline.SendAsync(new HttpMethod(method), "/things");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        keys.Should().HaveCount(2);
        keys[0].Should().NotBeNullOrEmpty();
        keys[1].Should().Be(keys[0]);
    }

    [Fact]
    public async Task SendMultipartAsync_Retry_ReusesSameIdempotencyKey()
    {
        var keys = CaptureKeysWith500Then200();
        using var pipeline = CreatePipeline();
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        var response = await pipeline.SendMultipartAsync(
            "/upload", stream, "f.bin", "application/octet-stream");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        keys.Should().HaveCount(2);
        keys[0].Should().NotBeNullOrEmpty();
        keys[1].Should().Be(keys[0]);
    }
}
