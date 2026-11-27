using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;
using Trix.Models;

namespace Trix.Tests;

/// <summary>
/// Tests for resource operations including pagination and streaming.
/// </summary>
public class ResourceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly TrixClient _client;

    public ResourceTests()
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

    #region Pagination Tests

    [Fact]
    public async Task ListAllAsync_MultiplePages_YieldsAllItems()
    {
        // Arrange
        var page1Response = """
        {
            "data": [
                { "id": "mem_1", "content": "Content 1", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" },
                { "id": "mem_2", "content": "Content 2", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" }
            ],
            "pagination": { "total": 4, "page": 1, "limit": 2, "has_more": true }
        }
        """;

        var page2Response = """
        {
            "data": [
                { "id": "mem_3", "content": "Content 3", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" },
                { "id": "mem_4", "content": "Content 4", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" }
            ],
            "pagination": { "total": 4, "page": 2, "limit": 2, "has_more": false }
        }
        """;

        var callCount = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                var content = callCount == 1 ? page1Response : page2Response;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
            });

        // Act
        var memories = new List<Memory>();
        await foreach (var memory in _client.Memories.ListAllAsync(new ListMemoriesRequest { Limit = 2 }))
        {
            memories.Add(memory);
        }

        // Assert
        memories.Should().HaveCount(4);
        memories.Select(m => m.Id).Should().BeEquivalentTo(new[] { "mem_1", "mem_2", "mem_3", "mem_4" });
        callCount.Should().Be(2);
    }

    [Fact]
    public async Task ListAllAsync_EmptyResponse_ReturnsEmpty()
    {
        // Arrange
        var emptyResponse = """
        {
            "data": [],
            "pagination": { "total": 0, "page": 1, "limit": 100, "has_more": false }
        }
        """;

        SetupResponse(HttpStatusCode.OK, emptyResponse);

        // Act
        var memories = new List<Memory>();
        await foreach (var memory in _client.Memories.ListAllAsync())
        {
            memories.Add(memory);
        }

        // Assert
        memories.Should().BeEmpty();
    }

    [Fact]
    public async Task ListAllAsync_Cancellation_StopsIteration()
    {
        // Arrange - Each call returns a fresh response with fresh content stream
        var callCount = 0;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                var pageResponse = $$"""
                {
                    "data": [
                        { "id": "mem_{{callCount}}", "content": "Content {{callCount}}", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" }
                    ],
                    "pagination": { "total": 100, "page": {{callCount}}, "limit": 1, "has_more": true }
                }
                """;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(pageResponse, Encoding.UTF8, "application/json")
                };
            });

        using var cts = new CancellationTokenSource();
        var memories = new List<Memory>();

        // Act
        await foreach (var memory in _client.Memories.ListAllAsync(cancellationToken: cts.Token))
        {
            memories.Add(memory);
            if (memories.Count >= 2)
            {
                cts.Cancel();
                break;
            }
        }

        // Assert
        memories.Should().HaveCount(2);
    }

    #endregion

    #region Stream Tests

    [Fact]
    public async Task GetAudioAsync_ReturnsStream_DisposesResponseCorrectly()
    {
        // Arrange
        var audioData = new byte[] { 0x49, 0x44, 0x33 }; // MP3 header
        var responseContent = new ByteArrayContent(audioData);
        responseContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = responseContent
            });

        // Act
        using var stream = await _client.Memories.GetAudioAsync("mem_123");
        var buffer = new byte[3];
        var bytesRead = await stream.ReadAsync(buffer);

        // Assert
        bytesRead.Should().Be(3);
        buffer.Should().BeEquivalentTo(audioData);
    }

    #endregion

    #region CRUD Tests

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsMemory()
    {
        // Arrange
        var responseJson = """
        {
            "id": "mem_new",
            "content": "Test content",
            "type": "text",
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-01T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var memory = await _client.Memories.CreateAsync(new CreateMemoryRequest
        {
            Content = "Test content"
        });

        // Assert
        memory.Should().NotBeNull();
        memory.Id.Should().Be("mem_new");
        memory.Content.Should().Be("Test content");
    }

    [Fact]
    public async Task GetAsync_ValidId_ReturnsMemory()
    {
        // Arrange
        var responseJson = """
        {
            "id": "mem_123",
            "content": "Existing content",
            "type": "text",
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-01T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var memory = await _client.Memories.GetAsync("mem_123");

        // Assert
        memory.Should().NotBeNull();
        memory.Id.Should().Be("mem_123");
    }

    [Fact]
    public async Task GetAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Memories.GetAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Memories.GetAsync("");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteAsync_ValidId_Succeeds()
    {
        // Arrange
        SetupResponse(HttpStatusCode.NoContent, "");

        // Act & Assert
        var act = () => _client.Memories.DeleteAsync("mem_123");
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Helper Methods

    private void SetupResponse(HttpStatusCode statusCode, string content)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    #endregion
}
