using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;

namespace Trix.Tests;

/// <summary>
/// ADR-143 — Unit tests for TrixClient.PingAsync().
/// </summary>
public class PingTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly TrixClient _client;

    public PingTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        var options = new TrixClientOptions
        {
            ApiKey = "test_key",
            BaseUrl = "https://api.test.com",
            HttpHandler = _mockHandler.Object,
        };
        _client = new TrixClient(options);
    }

    public void Dispose() => _client.Dispose();

    private void SetupHealth(string body, HttpStatusCode status = HttpStatusCode.OK)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("/v1/health")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            });
    }

    [Fact]
    public async Task PingAsync_ReturnsOk_WhenServerReportsOk()
    {
        SetupHealth("{\"status\":\"ok\",\"version\":\"0.6.0\",\"uptime\":123.4,\"timestamp\":\"2026-04-16T09:00:00Z\"}");

        var result = await _client.PingAsync();

        result.Ok.Should().BeTrue();
        result.Version.Should().Be("0.6.0");
        result.LatencyMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task PingAsync_ReturnsNotOk_WhenStatusDegraded()
    {
        SetupHealth("{\"status\":\"degraded\",\"version\":\"0.6.0\"}");

        var result = await _client.PingAsync();

        result.Ok.Should().BeFalse();
        result.Version.Should().Be("0.6.0");
    }

    [Fact]
    public async Task PingAsync_ReturnsNotOk_WhenStatusMissing()
    {
        SetupHealth("{\"version\":\"0.6.0\"}");

        var result = await _client.PingAsync();

        result.Ok.Should().BeFalse();
        result.Version.Should().Be("0.6.0");
    }
}
