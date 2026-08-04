using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;
using Trix.Models;

namespace Trix.Tests;

/// <summary>
/// Tests for TasksResource, focused on the snake_case wire contract and the
/// non-standard list envelope / missing complete endpoint.
/// </summary>
public class TasksResourceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly TrixClient _client;

    private HttpMethod? _capturedMethod;
    private Uri? _capturedUri;
    private string? _capturedBody;

    public TasksResourceTests()
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

    private void Setup(HttpStatusCode statusCode, string content)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
            {
                _capturedMethod = req.Method;
                _capturedUri = req.RequestUri;
                // Read the body inside the callback: the pipeline disposes the
                // request (and its content) once SendAsync returns.
                _capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    private const string TaskJson = """
    {
        "id": "task_1",
        "title": "Write docs",
        "description": "Cover the tasks API",
        "status": "todo",
        "priority": 2,
        "space_id": "space_1",
        "due_at": "2026-08-05T12:00:00Z",
        "created_at": "2026-08-01T00:00:00Z",
        "updated_at": "2026-08-01T00:00:00Z",
        "version": 3,
        "labels": [
            { "id": "lbl_1", "name": "urgent", "color": "#ff0000", "account_id": "acc_1", "created_at": "2026-08-01T00:00:00Z" }
        ]
    }
    """;

    [Fact]
    public async Task CreateAsync_SendsSnakeCaseBody_AndParsesResponse()
    {
        // Arrange
        Setup(HttpStatusCode.Created, TaskJson);

        // Act
        var task = await _client.Tasks.CreateAsync(new CreateTaskRequest
        {
            Title = "Write docs",
            SpaceId = "space_1",
            DueAt = DateTimeOffset.Parse("2026-08-05T12:00:00Z"),
            ParentTaskId = "task_0",
            Priority = 2
        });

        // Assert - request maps to the snake_case wire contract
        _capturedMethod.Should().Be(HttpMethod.Post);
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks");
        _capturedBody.Should().Contain("\"space_id\":\"space_1\"");
        _capturedBody.Should().Contain("\"parent_task_id\":\"task_0\"");
        _capturedBody.Should().Contain("\"due_at\":");

        // Assert - response parses snake_case fields
        task.Id.Should().Be("task_1");
        task.SpaceId.Should().Be("space_1");
        task.Version.Should().Be(3);
        task.DueAt.Should().NotBeNull();
        task.Labels.Should().ContainSingle();
        task.Labels![0].Name.Should().Be("urgent");
        task.Labels[0].Color.Should().Be("#ff0000");
    }

    [Fact]
    public async Task GetAsync_WithInclude_AddsIncludeParam()
    {
        // Arrange
        Setup(HttpStatusCode.OK, TaskJson);

        // Act
        await _client.Tasks.GetAsync("task_1", include: new[] { "labels", "subtasks" });

        // Assert
        _capturedMethod.Should().Be(HttpMethod.Get);
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks/task_1");
        Uri.UnescapeDataString(_capturedUri.Query).Should().Contain("include=labels,subtasks");
    }

    [Fact]
    public async Task UpdateAsync_SendsVersionForOptimisticConcurrency()
    {
        // Arrange
        Setup(HttpStatusCode.OK, TaskJson);

        // Act
        await _client.Tasks.UpdateAsync("task_1", new UpdateTaskRequest
        {
            Title = "Updated",
            Version = 3
        });

        // Assert
        _capturedMethod.Should().Be(HttpMethod.Patch);
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks/task_1");
        _capturedBody.Should().Contain("\"version\":3");
    }

    [Fact]
    public async Task CompleteAsync_PatchesStatusDone()
    {
        // Arrange - there is no dedicated complete endpoint; it is a status update.
        Setup(HttpStatusCode.OK, TaskJson);

        // Act
        await _client.Tasks.CompleteAsync("task_1");

        // Assert
        _capturedMethod.Should().Be(HttpMethod.Patch);
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks/task_1");
        _capturedBody.Should().Contain("\"status\":\"done\"");
    }

    [Fact]
    public async Task DeleteAsync_SendsDelete()
    {
        // Arrange
        Setup(HttpStatusCode.NoContent, "{}");

        // Act
        await _client.Tasks.DeleteAsync("task_1");

        // Assert
        _capturedMethod.Should().Be(HttpMethod.Delete);
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks/task_1");
    }

    [Fact]
    public async Task ListAsync_ParsesEnvelope_AndBuildsSnakeCaseQuery()
    {
        // Arrange - the list endpoint returns { tasks, total, limit, offset }.
        var json = $$"""
        { "tasks": [ {{TaskJson}} ], "total": 5, "limit": 1, "offset": 0 }
        """;
        Setup(HttpStatusCode.OK, json);

        // Act
        var page = await _client.Tasks.ListAsync(new ListTasksRequest
        {
            SpaceId = "space_1",
            Status = TaskStatuses.Todo,
            Limit = 1
        });

        // Assert
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks");
        _capturedUri.Query.Should().Contain("space_id=space_1");
        _capturedUri.Query.Should().Contain("status=todo");
        page.Tasks.Should().ContainSingle();
        page.Total.Should().Be(5);
        page.HasMore.Should().BeTrue(); // offset(0) + count(1) < total(5)
    }

    [Fact]
    public async Task ListAsync_WithoutSpaceOrAssignee_Throws()
    {
        // Act & Assert - the API requires one of them; fail fast client-side.
        var act = () => _client.Tasks.ListAsync(new ListTasksRequest { Status = TaskStatuses.Todo });
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetSubtasksAsync_UnwrapsSubtasksEnvelope()
    {
        // Arrange
        var json = $$"""
        { "subtasks": [ {{TaskJson}} ] }
        """;
        Setup(HttpStatusCode.OK, json);

        // Act
        var subtasks = await _client.Tasks.GetSubtasksAsync("task_1");

        // Assert
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks/task_1/subtasks");
        subtasks.Should().ContainSingle();
        subtasks[0].Id.Should().Be("task_1");
    }

    [Fact]
    public async Task CreateSubtaskAsync_PostsToSubtasksPath()
    {
        // Arrange
        Setup(HttpStatusCode.Created, TaskJson);

        // Act
        await _client.Tasks.CreateSubtaskAsync("task_1", new CreateSubtaskRequest { Title = "Sub" });

        // Assert
        _capturedMethod.Should().Be(HttpMethod.Post);
        _capturedUri!.AbsolutePath.Should().Be("/v1/tasks/task_1/subtasks");
        _capturedBody.Should().Contain("\"title\":\"Sub\"");
    }

    [Fact]
    public void TasksResource_IsExposedOnClient()
    {
        _client.Tasks.Should().NotBeNull();
    }
}
