using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;
using Trix.Models;

namespace Trix.Tests;

/// <summary>
/// Tests for graph expansion functionality.
/// </summary>
public class GraphExpansionTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly TrixClient _client;

    public GraphExpansionTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.test.com")
        };

        var options = new TrixClientOptions
        {
            ApiKey = "test_key",
            BaseUrl = "https://api.test.com",
            HttpHandler = _mockHandler.Object
        };

        _client = new TrixClient(options);
    }

    public void Dispose()
    {
        _client.Dispose();
        _httpClient.Dispose();
    }

    [Fact]
    public async Task ExpandAsync_BasicExpansion_ReturnsResult()
    {
        // Arrange
        var responseJson = """
        {
            "seedMemories": ["mem_1", "mem_2"],
            "expandedMemories": [
                {
                    "id": "mem_3",
                    "spaceId": "space_1",
                    "content": "Related memory",
                    "type": "text",
                    "tags": [],
                    "metadata": {},
                    "createdAt": "2024-01-01T00:00:00Z",
                    "updatedAt": "2024-01-01T00:00:00Z"
                }
            ],
            "relationships": [
                {
                    "id": "rel_1",
                    "source_id": "mem_1",
                    "target_id": "mem_3",
                    "relationship_type": "related_to",
                    "weight": 0.8,
                    "created_at": "2024-01-01T00:00:00Z",
                    "updated_at": "2024-01-01T00:00:00Z"
                }
            ],
            "stats": {
                "seedCount": 2,
                "expandedCount": 1,
                "finalCount": 1,
                "relationshipsFound": 1,
                "hopsUsed": 1
            },
            "scoring": null
        }
        """;

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.PathAndQuery == "/v1/graph/expand"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.Graph.ExpandAsync(new ExpandGraphRequest
        {
            SeedMemoryIds = new List<string> { "mem_1", "mem_2" }
        });

        // Assert
        result.Should().NotBeNull();
        result.SeedMemories.Should().BeEquivalentTo(new[] { "mem_1", "mem_2" });
        result.ExpandedMemories.Should().HaveCount(1);
        result.ExpandedMemories[0].Id.Should().Be("mem_3");
        result.Relationships.Should().HaveCount(1);
        result.Stats.SeedCount.Should().Be(2);
        result.Stats.ExpandedCount.Should().Be(1);
        result.Scoring.Should().BeNull();
    }

    [Fact]
    public async Task ExpandAsync_WithAllParameters_SendsCorrectRequest()
    {
        // Arrange
        var responseJson = """
        {
            "seedMemories": ["mem_1"],
            "expandedMemories": [],
            "relationships": [],
            "stats": {
                "seedCount": 1,
                "expandedCount": 0,
                "finalCount": 0
            },
            "scoring": null
        }
        """;

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.Graph.ExpandAsync(new ExpandGraphRequest
        {
            SeedMemoryIds = new List<string> { "mem_1" },
            MaxHops = 3,
            MinWeight = 0.5,
            RelationshipTypes = new List<string> { "related_to", "supports" },
            Direction = "outgoing",
            IncludeContent = false,
            ApplyHybridScoring = false
        });

        // Assert
        result.Should().NotBeNull();
        result.SeedMemories.Should().BeEquivalentTo(new[] { "mem_1" });
        result.ExpandedMemories.Should().BeEmpty();
    }

    [Fact]
    public async Task ExpandAsync_WithHybridScoring_ReturnsScoring()
    {
        // Arrange
        var responseJson = """
        {
            "seedMemories": ["mem_1"],
            "expandedMemories": [
                {
                    "id": "mem_2",
                    "spaceId": "space_1",
                    "content": "Scored memory",
                    "type": "text",
                    "tags": [],
                    "metadata": {},
                    "createdAt": "2024-01-01T00:00:00Z",
                    "updatedAt": "2024-01-01T00:00:00Z",
                    "score": 0.85
                }
            ],
            "relationships": [],
            "stats": {
                "seedCount": 1,
                "expandedCount": 1,
                "finalCount": 1
            },
            "scoring": {
                "applied": true,
                "weights": {
                    "semantic": 0.4,
                    "graph": 0.3,
                    "coActivation": 0.2,
                    "recency": 0.05,
                    "salience": 0.05
                }
            }
        }
        """;

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.Graph.ExpandAsync(new ExpandGraphRequest
        {
            SeedMemoryIds = new List<string> { "mem_1" },
            ApplyHybridScoring = true
        });

        // Assert
        result.Should().NotBeNull();
        result.Scoring.Should().NotBeNull();
        result.Scoring!.Applied.Should().BeTrue();
        result.Scoring.Weights.Should().NotBeNull();
        result.Scoring.Weights!.Semantic.Should().Be(0.4);
        result.Scoring.Weights.Graph.Should().Be(0.3);
        result.Scoring.Weights.CoActivation.Should().Be(0.2);
    }

    [Fact]
    public async Task ExpandAsync_EmptySeedMemories_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _client.Graph.ExpandAsync(new ExpandGraphRequest
            {
                SeedMemoryIds = new List<string>()
            }));
    }

    [Fact]
    public async Task ExpandAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _client.Graph.ExpandAsync(null!));
    }
}
