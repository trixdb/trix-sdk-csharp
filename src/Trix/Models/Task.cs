using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// A task.
/// </summary>
/// <remarks>
/// Named <c>TrixTask</c> to avoid colliding with
/// <see cref="System.Threading.Tasks.Task"/>. The tasks API uses a
/// <c>snake_case</c> wire contract (unlike most Trix resources), which is why
/// every property carries an explicit <see cref="JsonPropertyNameAttribute"/>.
/// </remarks>
public class TrixTask
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the task title.</summary>
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    /// <summary>Gets or sets the task description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the status (see <see cref="TaskStatuses"/>).</summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = TaskStatuses.Todo;

    /// <summary>Gets or sets the priority (1 = highest .. 5 = lowest).</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Gets or sets the space this task belongs to.</summary>
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the section this task belongs to.</summary>
    [JsonPropertyName("section_id")]
    public string? SectionId { get; set; }

    /// <summary>Gets or sets the project this task belongs to.</summary>
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    /// <summary>Gets or sets the parent task ID (for subtasks).</summary>
    [JsonPropertyName("parent_task_id")]
    public string? ParentTaskId { get; set; }

    /// <summary>Gets or sets the assignee ID.</summary>
    [JsonPropertyName("assignee_id")]
    public string? AssigneeId { get; set; }

    /// <summary>Gets or sets the assignee type (see <see cref="TaskAssigneeTypes"/>).</summary>
    [JsonPropertyName("assignee_type")]
    public string? AssigneeType { get; set; }

    /// <summary>Gets or sets the due timestamp.</summary>
    [JsonPropertyName("due_at")]
    public DateTimeOffset? DueAt { get; set; }

    /// <summary>Gets or sets the completion timestamp.</summary>
    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>Gets or sets the identifier of the creator.</summary>
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Gets or sets the optimistic-concurrency version.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>Gets or sets the labels attached to this task.</summary>
    [JsonPropertyName("labels")]
    public List<Label>? Labels { get; set; }
}

/// <summary>
/// A task label.
/// </summary>
public class Label
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the label name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the label color (hex or named).</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>Gets or sets the space this label belongs to.</summary>
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the owning account.</summary>
    [JsonPropertyName("account_id")]
    public string? AccountId { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Known task status values.
/// </summary>
public static class TaskStatuses
{
    /// <summary>Not started.</summary>
    public const string Todo = "todo";

    /// <summary>In progress.</summary>
    public const string InProgress = "in_progress";

    /// <summary>Completed.</summary>
    public const string Done = "done";

    /// <summary>Cancelled.</summary>
    public const string Cancelled = "cancelled";
}

/// <summary>
/// Known task assignee types.
/// </summary>
public static class TaskAssigneeTypes
{
    /// <summary>A human user.</summary>
    public const string User = "user";

    /// <summary>An agent/bot.</summary>
    public const string Agent = "agent";
}

/// <summary>
/// Parameters for creating a task. Requires <see cref="Title"/>; a task should
/// belong to a space (<see cref="SpaceId"/>) unless created as a subtask.
/// </summary>
public class CreateTaskRequest
{
    /// <summary>Gets or sets the task title (required).</summary>
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the priority (1..5).</summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>Gets or sets the status (see <see cref="TaskStatuses"/>).</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>Gets or sets the due timestamp.</summary>
    [JsonPropertyName("due_at")]
    public DateTimeOffset? DueAt { get; set; }

    /// <summary>Gets or sets the parent task ID.</summary>
    [JsonPropertyName("parent_task_id")]
    public string? ParentTaskId { get; set; }

    /// <summary>Gets or sets the section ID.</summary>
    [JsonPropertyName("section_id")]
    public string? SectionId { get; set; }

    /// <summary>Gets or sets the project ID.</summary>
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    /// <summary>Gets or sets the label IDs to attach.</summary>
    [JsonPropertyName("labels")]
    public List<string>? Labels { get; set; }
}

/// <summary>
/// Parameters for updating a task. Set <see cref="Version"/> to opt into
/// optimistic concurrency (the API returns 409 on a version mismatch).
/// </summary>
public class UpdateTaskRequest
{
    /// <summary>Gets or sets the title.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the status (see <see cref="TaskStatuses"/>).</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>Gets or sets the priority (1..5).</summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>Gets or sets the due timestamp.</summary>
    [JsonPropertyName("due_at")]
    public DateTimeOffset? DueAt { get; set; }

    /// <summary>Gets or sets the assignee ID.</summary>
    [JsonPropertyName("assignee_id")]
    public string? AssigneeId { get; set; }

    /// <summary>Gets or sets the assignee type (see <see cref="TaskAssigneeTypes"/>).</summary>
    [JsonPropertyName("assignee_type")]
    public string? AssigneeType { get; set; }

    /// <summary>Gets or sets the section ID.</summary>
    [JsonPropertyName("section_id")]
    public string? SectionId { get; set; }

    /// <summary>Gets or sets the project ID.</summary>
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    /// <summary>Gets or sets the label IDs (replaces the existing set).</summary>
    [JsonPropertyName("labels")]
    public List<string>? Labels { get; set; }

    /// <summary>Gets or sets the expected version for optimistic concurrency.</summary>
    [JsonPropertyName("version")]
    public int? Version { get; set; }
}

/// <summary>
/// Parameters for creating a subtask under a parent task.
/// </summary>
public class CreateSubtaskRequest
{
    /// <summary>Gets or sets the subtask title (required).</summary>
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the priority (1..5).</summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>Gets or sets the status (see <see cref="TaskStatuses"/>).</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>Gets or sets the due timestamp.</summary>
    [JsonPropertyName("due_at")]
    public DateTimeOffset? DueAt { get; set; }

    /// <summary>Gets or sets the label IDs to attach.</summary>
    [JsonPropertyName("labels")]
    public List<string>? Labels { get; set; }
}

/// <summary>
/// Parameters for listing tasks. The API requires either <see cref="SpaceId"/>
/// or <see cref="AssigneeId"/> to scope the query.
/// </summary>
public class ListTasksRequest
{
    /// <summary>Filter by space ID.</summary>
    public string? SpaceId { get; set; }

    /// <summary>Filter by assignee ID.</summary>
    public string? AssigneeId { get; set; }

    /// <summary>Filter by assignee type (see <see cref="TaskAssigneeTypes"/>).</summary>
    public string? AssigneeType { get; set; }

    /// <summary>Filter by status (see <see cref="TaskStatuses"/>).</summary>
    public string? Status { get; set; }

    /// <summary>Filter by priority (1..5).</summary>
    public int? Priority { get; set; }

    /// <summary>Filter by parent task ID.</summary>
    public string? ParentTaskId { get; set; }

    /// <summary>Filter by project ID.</summary>
    public string? ProjectId { get; set; }

    /// <summary>Only include tasks due before this timestamp.</summary>
    public DateTimeOffset? DueBefore { get; set; }

    /// <summary>Only include tasks due after this timestamp.</summary>
    public DateTimeOffset? DueAfter { get; set; }

    /// <summary>Include completed tasks (default: server-side false).</summary>
    public bool? IncludeCompleted { get; set; }

    /// <summary>Maximum number of results (1..100).</summary>
    public int? Limit { get; set; }

    /// <summary>Offset for pagination.</summary>
    public int? Offset { get; set; }
}

/// <summary>
/// A page of tasks returned by the list endpoint.
/// </summary>
public class TaskList
{
    /// <summary>Gets or sets the tasks in this page.</summary>
    [JsonPropertyName("tasks")]
    public required List<TrixTask> Tasks { get; set; }

    /// <summary>Gets or sets the total number of matching tasks.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>Gets or sets the page size.</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>Gets or sets the page offset.</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>Gets whether more tasks exist beyond this page.</summary>
    [JsonIgnore]
    public bool HasMore => Offset + Tasks.Count < Total;
}
