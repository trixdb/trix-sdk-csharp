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
/// ADR-109a observability (tick 79) — Unit tests for
/// AgentResource.ResolvePipelineAsync().
/// </summary>
public class AgentPipelineResolveTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly TrixClient _client;

    public AgentPipelineResolveTests()
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

    private void SetupResolve(string body, string expectedPath)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith(expectedPath)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            });
    }

    [Fact]
    public async Task ResolvePipelineAsync_NoArgs_HitsResolveEndpoint()
    {
        SetupResolve(
            "{\"name\":null,\"source\":null,\"preset\":null}",
            "/v1/pipeline-presets/_resolve");

        var result = await _client.Agent.ResolvePipelineAsync();

        result.Should().NotBeNull();
        result.Name.Should().BeNull();
        result.Source.Should().BeNull();
    }

    [Fact]
    public async Task ResolvePipelineAsync_WithPipeline_SendsQueryParam()
    {
        SetupResolve(
            "{\"name\":\"longmem-v1\",\"source\":\"caller\",\"preset\":{\"name\":\"longmem-v1\"}}",
            "/v1/pipeline-presets/_resolve?pipeline=longmem-v1");

        var result = await _client.Agent.ResolvePipelineAsync(pipeline: "longmem-v1");

        result.Name.Should().Be("longmem-v1");
        result.Source.Should().Be("caller");
    }

    [Fact]
    public async Task ResolvePipelineAsync_WithBothArgs_SendsBothParams()
    {
        SetupResolve(
            "{\"name\":\"space-pref\",\"source\":\"space\",\"preset\":{\"name\":\"space-pref\"}}",
            "/v1/pipeline-presets/_resolve?space_id=space-123&pipeline=caller");

        var result = await _client.Agent.ResolvePipelineAsync(
            spaceId: "space-123",
            pipeline: "caller");

        result.Source.Should().Be("space");
    }

}
