using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;
using Trix.Models;

namespace Trix.Tests;

/// <summary>
/// Tests for MemoriesResource, focused on the MQL query path (QueryAsync).
/// </summary>
public class MemoriesResourceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly TrixClient _client;

    public MemoriesResourceTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        var options = new TrixClientOptions
        {
            ApiKey = "test_key",
            BaseUrl = "https://api.test.com",
            HttpHandler = _mockHandler.Object
        };
        _client = new TrixClient(options);
    }

    public void Dispose() => _client.Dispose();

    private HttpRequestMessage? SetupCapture(string content)
    {
        HttpRequestMessage? captured = null;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
        return captured;
    }

    [Fact]
    public async Task QueryAsync_Sends_Mql_And_Returns_Memories()
    {
        var json = """
        {
            "data": [{ "id": "mem_1", "content": "a fact", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" }],
            "pagination": { "total": 1, "limit": 25, "hasMore": false }
        }
        """;
        HttpRequestMessage? captured = null;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var result = await _client.Memories.QueryAsync("type:fact quality>0.8", limit: 25, offset: 5);

        result.IsAggregate.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(1);
        result.Data![0].Id.Should().Be("mem_1");

        var query = Uri.UnescapeDataString(captured!.RequestUri!.Query);
        query.Should().Contain("mql=type:fact quality>0.8");
        query.Should().Contain("limit=25");
        query.Should().Contain("offset=5");
    }

    [Fact]
    public async Task QueryAsync_Detects_Aggregate_Shape()
    {
        var json = """
        {
            "aggregate": [{ "group": "fact", "count": 3 }],
            "group_by": "type",
            "metrics": ["count"]
        }
        """;
        _ = SetupCapture(json);

        var result = await _client.Memories.QueryAsync("group by type count");

        result.IsAggregate.Should().BeTrue();
        result.GroupBy.Should().Be("type");
        result.Aggregate.Should().NotBeNull();
        result.Aggregate!.Should().HaveCount(1);
        result.Metrics.Should().ContainSingle().Which.Should().Be("count");
    }
}
