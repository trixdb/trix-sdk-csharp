using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;
using Trix.Models;

namespace Trix.Tests;

/// <summary>
/// Tests for graph traversal functionality.
/// </summary>
public class GraphTraversalTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly TrixClient _client;

    public GraphTraversalTests()
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
    public async Task TraverseAsync_SingleStartNode_ReturnsResult()
    {
        // Arrange
        var responseJson = """
        {
            "nodes": [
                {
                    "id": "mem_1",
                    "type": "memory",
                    "data": {},
                    "depth": 0
                },
                {
                    "id": "mem_2",
                    "type": "memory",
                    "data": {},
                    "depth": 1
                }
            ],
            "edges": [
                {
                    "source": "mem_1",
                    "target": "mem_2",
                    "relationshipType": "related_to",
                    "strength": 0.8
                }
            ]
        }
        """;

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.PathAndQuery == "/v1/graph/traverse"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.Graph.TraverseAsync(new TraverseRequest
        {
            StartNodeIds = new List<string> { "mem_1" },
            MaxDepth = 2
        });

        // Assert
        result.Should().NotBeNull();
        result.Nodes.Should().HaveCount(2);
        result.Edges.Should().HaveCount(1);
        result.Nodes[0].Id.Should().Be("mem_1");
        result.Nodes[1].Id.Should().Be("mem_2");
    }

    [Fact]
    public async Task TraverseAsync_MultipleStartNodes_ReturnsResult()
    {
        // Arrange
        var responseJson = """
        {
            "nodes": [
                {
                    "id": "mem_1",
                    "type": "memory",
                    "data": {},
                    "depth": 0
                },
                {
                    "id": "mem_2",
                    "type": "memory",
                    "data": {},
                    "depth": 0
                },
                {
                    "id": "mem_3",
                    "type": "memory",
                    "data": {},
                    "depth": 1
                }
            ],
            "edges": [
                {
                    "source": "mem_1",
                    "target": "mem_3",
                    "relationshipType": "related_to",
                    "strength": 0.8
                },
                {
                    "source": "mem_2",
                    "target": "mem_3",
                    "relationshipType": "related_to",
                    "strength": 0.7
                }
            ]
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
        var result = await _client.Graph.TraverseAsync(new TraverseRequest
        {
            StartNodeIds = new List<string> { "mem_1", "mem_2" },
            MaxDepth = 2
        });

        // Assert
        result.Should().NotBeNull();
        result.Nodes.Should().HaveCount(3);
        result.Edges.Should().HaveCount(2);
        result.Nodes[0].Id.Should().Be("mem_1");
        result.Nodes[1].Id.Should().Be("mem_2");
        result.Nodes[2].Id.Should().Be("mem_3");
    }

    [Fact]
    public async Task TraverseAsync_EmptyStartNodes_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _client.Graph.TraverseAsync(new TraverseRequest
            {
                StartNodeIds = new List<string>()
            }));
    }

    [Fact]
    public async Task TraverseAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _client.Graph.TraverseAsync(null!));
    }
}
