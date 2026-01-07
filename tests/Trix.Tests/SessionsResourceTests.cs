using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;
using Trix.Models;

namespace Trix.Tests;

/// <summary>
/// Tests for SessionsResource operations.
/// </summary>
public class SessionsResourceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly TrixClient _client;

    public SessionsResourceTests()
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

    #region Create Tests

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsSession()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_123",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Test Session",
            "description": "A test session",
            "type": "conversation",
            "status": "active",
            "retentionPolicy": "permanent",
            "isPrivate": false,
            "messageCount": 0,
            "memoryCount": 0,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-01T00:00:00Z",
            "lastActiveAt": "2025-01-01T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.CreateAsync(new CreateCliSessionRequest
        {
            Name = "Test Session",
            Description = "A test session"
        });

        // Assert
        session.Should().NotBeNull();
        session.Id.Should().Be("sess_123");
        session.Name.Should().Be("Test Session");
        session.Type.Should().Be(SessionType.Conversation);
        session.Status.Should().Be(SessionStatus.Active);
    }

    [Fact]
    public async Task CreateAsync_WithSimpleParams_ReturnsSession()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_124",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Simple Session",
            "type": "conversation",
            "status": "active",
            "retentionPolicy": "permanent",
            "isPrivate": false,
            "messageCount": 0,
            "memoryCount": 0,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-01T00:00:00Z",
            "lastActiveAt": "2025-01-01T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.CreateAsync("Simple Session");

        // Assert
        session.Should().NotBeNull();
        session.Id.Should().Be("sess_124");
        session.Name.Should().Be("Simple Session");
    }

    [Fact]
    public async Task CreateAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _client.Sessions.CreateAsync((CreateCliSessionRequest)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region Get Tests

    [Fact]
    public async Task GetAsync_ValidId_ReturnsSession()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_123",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Existing Session",
            "type": "project",
            "status": "active",
            "retentionPolicy": "permanent",
            "isPrivate": false,
            "messageCount": 5,
            "memoryCount": 10,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-01T00:00:00Z",
            "lastActiveAt": "2025-01-01T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.GetAsync("sess_123");

        // Assert
        session.Should().NotBeNull();
        session.Id.Should().Be("sess_123");
        session.Name.Should().Be("Existing Session");
        session.Type.Should().Be(SessionType.Project);
        session.MessageCount.Should().Be(5);
        session.MemoryCount.Should().Be(10);
    }

    [Fact]
    public async Task GetAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.GetAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.GetAsync("");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region List Tests

    [Fact]
    public async Task ListAsync_NoFilters_ReturnsAllSessions()
    {
        // Arrange
        var responseJson = """
        {
            "data": [
                {
                    "id": "sess_1",
                    "accountId": "acc_456",
                    "userId": "user_789",
                    "createdBy": "user_789",
                    "name": "Session 1",
                    "type": "conversation",
                    "status": "active",
                    "retentionPolicy": "permanent",
                    "isPrivate": false,
                    "messageCount": 0,
                    "memoryCount": 0,
                    "createdAt": "2025-01-01T00:00:00Z",
                    "updatedAt": "2025-01-01T00:00:00Z",
                    "lastActiveAt": "2025-01-01T00:00:00Z"
                },
                {
                    "id": "sess_2",
                    "accountId": "acc_456",
                    "userId": "user_789",
                    "createdBy": "user_789",
                    "name": "Session 2",
                    "type": "project",
                    "status": "paused",
                    "retentionPolicy": "permanent",
                    "isPrivate": false,
                    "messageCount": 0,
                    "memoryCount": 0,
                    "createdAt": "2025-01-01T00:00:00Z",
                    "updatedAt": "2025-01-01T00:00:00Z",
                    "lastActiveAt": "2025-01-01T00:00:00Z"
                }
            ],
            "pagination": {
                "total": 2,
                "page": 1,
                "limit": 100,
                "hasMore": false
            }
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var response = await _client.Sessions.ListAsync();

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(2);
        response.Data[0].Id.Should().Be("sess_1");
        response.Data[1].Id.Should().Be("sess_2");
    }

    [Fact]
    public async Task ListAsync_WithFilters_ReturnsFilteredSessions()
    {
        // Arrange
        var responseJson = """
        {
            "data": [
                {
                    "id": "sess_active",
                    "accountId": "acc_456",
                    "userId": "user_789",
                    "createdBy": "user_789",
                    "name": "Active Session",
                    "type": "conversation",
                    "status": "active",
                    "retentionPolicy": "permanent",
                    "isPrivate": false,
                    "messageCount": 0,
                    "memoryCount": 0,
                    "createdAt": "2025-01-01T00:00:00Z",
                    "updatedAt": "2025-01-01T00:00:00Z",
                    "lastActiveAt": "2025-01-01T00:00:00Z"
                }
            ],
            "pagination": {
                "total": 1,
                "page": 1,
                "limit": 100,
                "hasMore": false
            }
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var response = await _client.Sessions.ListAsync(new ListCliSessionsParams
        {
            Status = SessionStatus.Active,
            Type = SessionType.Conversation,
            Limit = 10
        });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(1);
        response.Data[0].Status.Should().Be(SessionStatus.Active);
    }

    #endregion

    #region GetActive Tests

    [Fact]
    public async Task GetActiveAsync_ReturnsActiveSessions()
    {
        // Arrange
        var responseJson = """
        {
            "data": [
                {
                    "id": "sess_active_1",
                    "accountId": "acc_456",
                    "userId": "user_789",
                    "createdBy": "user_789",
                    "name": "Active Session 1",
                    "type": "conversation",
                    "status": "active",
                    "retentionPolicy": "permanent",
                    "isPrivate": false,
                    "messageCount": 0,
                    "memoryCount": 0,
                    "createdAt": "2025-01-01T00:00:00Z",
                    "updatedAt": "2025-01-01T00:00:00Z",
                    "lastActiveAt": "2025-01-01T00:00:00Z"
                }
            ],
            "pagination": {
                "total": 1,
                "page": 1,
                "limit": 100,
                "hasMore": false
            }
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var response = await _client.Sessions.GetActiveAsync();

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(1);
        response.Data[0].Status.Should().Be(SessionStatus.Active);
    }

    #endregion

    #region GetStats Tests

    [Fact]
    public async Task GetStatsAsync_ReturnsStatistics()
    {
        // Arrange
        var responseJson = """
        {
            "total": 100,
            "active": 20,
            "paused": 15,
            "completed": 50,
            "archived": 15,
            "byType": {
                "conversation": 40,
                "project": 30,
                "task": 20,
                "temporary": 10
            },
            "totalMessages": 5000,
            "totalMemories": 3000,
            "avgDuration": 3600.5
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var stats = await _client.Sessions.GetStatsAsync();

        // Assert
        stats.Should().NotBeNull();
        stats.Total.Should().Be(100);
        stats.Active.Should().Be(20);
        stats.Paused.Should().Be(15);
        stats.Completed.Should().Be(50);
        stats.ByType.Should().ContainKey("conversation");
        stats.ByType!["conversation"].Should().Be(40);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedSession()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_123",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Updated Session",
            "description": "Updated description",
            "type": "conversation",
            "status": "active",
            "retentionPolicy": "permanent",
            "isPrivate": true,
            "messageCount": 0,
            "memoryCount": 0,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-02T00:00:00Z",
            "lastActiveAt": "2025-01-02T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.UpdateAsync("sess_123", new UpdateCliSessionRequest
        {
            Name = "Updated Session",
            Description = "Updated description",
            IsPrivate = true
        });

        // Assert
        session.Should().NotBeNull();
        session.Name.Should().Be("Updated Session");
        session.Description.Should().Be("Updated description");
        session.IsPrivate.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.UpdateAsync(null!, new UpdateCliSessionRequest());
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _client.Sessions.UpdateAsync("sess_123", null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteAsync_ValidId_Succeeds()
    {
        // Arrange
        SetupResponse(HttpStatusCode.NoContent, "");

        // Act & Assert
        var act = () => _client.Sessions.DeleteAsync("sess_123");
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.DeleteAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region Pause Tests

    [Fact]
    public async Task PauseAsync_ValidId_ReturnsSessionWithPausedStatus()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_123",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Test Session",
            "type": "conversation",
            "status": "paused",
            "retentionPolicy": "permanent",
            "isPrivate": false,
            "messageCount": 5,
            "memoryCount": 3,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-02T00:00:00Z",
            "lastActiveAt": "2025-01-02T00:00:00Z",
            "pausedAt": "2025-01-02T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.PauseAsync("sess_123");

        // Assert
        session.Should().NotBeNull();
        session.Status.Should().Be(SessionStatus.Paused);
        session.PausedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task PauseAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.PauseAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region Resume Tests

    [Fact]
    public async Task ResumeAsync_ValidId_ReturnsSessionWithActiveStatus()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_123",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Test Session",
            "type": "conversation",
            "status": "active",
            "retentionPolicy": "permanent",
            "isPrivate": false,
            "messageCount": 5,
            "memoryCount": 3,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-02T00:00:00Z",
            "lastActiveAt": "2025-01-02T00:00:00Z",
            "pausedAt": null
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.ResumeAsync("sess_123");

        // Assert
        session.Should().NotBeNull();
        session.Status.Should().Be(SessionStatus.Active);
    }

    [Fact]
    public async Task ResumeAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.ResumeAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region Complete Tests

    [Fact]
    public async Task CompleteAsync_WithSummary_ReturnsCompletedSession()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_123",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Test Session",
            "type": "conversation",
            "status": "completed",
            "summary": "Session completed successfully",
            "retentionPolicy": "permanent",
            "isPrivate": false,
            "messageCount": 10,
            "memoryCount": 5,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-02T00:00:00Z",
            "lastActiveAt": "2025-01-02T00:00:00Z",
            "completedAt": "2025-01-02T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.CompleteAsync("sess_123", new CompleteCliSessionRequest
        {
            Summary = "Session completed successfully"
        });

        // Assert
        session.Should().NotBeNull();
        session.Status.Should().Be(SessionStatus.Completed);
        session.Summary.Should().Be("Session completed successfully");
        session.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CompleteAsync_WithoutSummary_ReturnsCompletedSession()
    {
        // Arrange
        var responseJson = """
        {
            "id": "sess_123",
            "accountId": "acc_456",
            "userId": "user_789",
            "createdBy": "user_789",
            "name": "Test Session",
            "type": "conversation",
            "status": "completed",
            "retentionPolicy": "permanent",
            "isPrivate": false,
            "messageCount": 10,
            "memoryCount": 5,
            "createdAt": "2025-01-01T00:00:00Z",
            "updatedAt": "2025-01-02T00:00:00Z",
            "lastActiveAt": "2025-01-02T00:00:00Z",
            "completedAt": "2025-01-02T00:00:00Z"
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var session = await _client.Sessions.CompleteAsync("sess_123");

        // Assert
        session.Should().NotBeNull();
        session.Status.Should().Be(SessionStatus.Completed);
        session.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CompleteAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.CompleteAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region GetMemories Tests

    [Fact]
    public async Task GetMemoriesAsync_ValidId_ReturnsMemories()
    {
        // Arrange
        var responseJson = """
        {
            "data": [
                {
                    "id": "mem_1",
                    "content": "Memory 1",
                    "type": "text",
                    "createdAt": "2025-01-01T00:00:00Z",
                    "updatedAt": "2025-01-01T00:00:00Z"
                },
                {
                    "id": "mem_2",
                    "content": "Memory 2",
                    "type": "text",
                    "createdAt": "2025-01-01T00:00:00Z",
                    "updatedAt": "2025-01-01T00:00:00Z"
                }
            ],
            "pagination": {
                "total": 2,
                "page": 1,
                "limit": 100,
                "hasMore": false
            }
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var response = await _client.Sessions.GetMemoriesAsync("sess_123");

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(2);
        response.Data[0].Id.Should().Be("mem_1");
        response.Data[1].Id.Should().Be("mem_2");
    }

    [Fact]
    public async Task GetMemoriesAsync_WithPagination_ReturnsPagedMemories()
    {
        // Arrange
        var responseJson = """
        {
            "data": [
                {
                    "id": "mem_1",
                    "content": "Memory 1",
                    "type": "text",
                    "createdAt": "2025-01-01T00:00:00Z",
                    "updatedAt": "2025-01-01T00:00:00Z"
                }
            ],
            "pagination": {
                "total": 10,
                "page": 1,
                "limit": 1,
                "hasMore": true
            }
        }
        """;
        SetupResponse(HttpStatusCode.OK, responseJson);

        // Act
        var response = await _client.Sessions.GetMemoriesAsync("sess_123", limit: 1, page: 1);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(1);
        response.Pagination!.HasMore.Should().BeTrue();
    }

    [Fact]
    public async Task GetMemoriesAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _client.Sessions.GetMemoriesAsync(null!);
        await act.Should().ThrowAsync<ArgumentException>();
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
