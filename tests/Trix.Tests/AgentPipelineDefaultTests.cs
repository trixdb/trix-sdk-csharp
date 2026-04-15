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
/// ADR-109a — Unit tests for the account-default pipeline preset methods
/// on AgentResource.
/// </summary>
public class AgentPipelineDefaultTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly TrixClient _client;

    public AgentPipelineDefaultTests()
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

    private void SetupResponse(HttpMethod expectedMethod, string expectedPathContains, string body, HttpStatusCode status = HttpStatusCode.OK)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == expectedMethod &&
                    req.RequestUri!.ToString().Contains(expectedPathContains)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(status)
            {
                Content = string.IsNullOrEmpty(body)
                    ? null
                    : new StringContent(body, Encoding.UTF8, "application/json"),
            });
    }

    [Fact]
    public async Task GetDefaultPipelineAsync_ReturnsName_WhenSet()
    {
        SetupResponse(HttpMethod.Get, "/pipeline-presets/_default", "{\"name\":\"longmem-v1\"}");

        var result = await _client.Agent.GetDefaultPipelineAsync();

        result.Should().Be("longmem-v1");
    }

    [Fact]
    public async Task GetDefaultPipelineAsync_ReturnsNull_WhenUnset()
    {
        SetupResponse(HttpMethod.Get, "/pipeline-presets/_default", "{\"name\":null}");

        var result = await _client.Agent.GetDefaultPipelineAsync();

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetDefaultPipelineAsync_PostsToCorrectPath_AndReturnsName()
    {
        SetupResponse(HttpMethod.Post, "/pipeline-presets/longmem-v1/set-default", "{\"name\":\"longmem-v1\"}");

        var result = await _client.Agent.SetDefaultPipelineAsync("longmem-v1");

        result.Should().Be("longmem-v1");
    }

    [Fact]
    public async Task SetDefaultPipelineAsync_ThrowsOnEmptyName()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _client.Agent.SetDefaultPipelineAsync(string.Empty));
    }

    [Fact]
    public async Task ClearDefaultPipelineAsync_SendsDelete()
    {
        SetupResponse(HttpMethod.Delete, "/pipeline-presets/_default", "", HttpStatusCode.NoContent);

        // Should not throw.
        await _client.Agent.ClearDefaultPipelineAsync();
    }
}
