using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Provides methods for managing tasks.
/// </summary>
/// <remarks>
/// The tasks API uses a <c>snake_case</c> wire contract, mapped by the explicit
/// <see cref="JsonPropertyNameAttribute"/> annotations on the task models.
/// </remarks>
public class TasksResource : BaseResource
{
    private const string BasePath = "/v1/tasks";

    internal TasksResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">The task creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task.</returns>
    public virtual async Task<TrixTask> CreateAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<TrixTask>(BasePath, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a task by ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="include">
    /// Optional relations to expand (e.g. <c>labels</c>, <c>links</c>, <c>subtasks</c>).
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    public virtual async Task<TrixTask> GetAsync(
        string id,
        IEnumerable<string>? include = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        var queryParams = new Dictionary<string, string?>();
        if (include != null)
        {
            var joined = string.Join(",", include);
            if (joined.Length > 0) queryParams["include"] = joined;
        }
        return await GetAsync<TrixTask>(
            $"{BasePath}/{Uri.EscapeDataString(id)}", queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task.</returns>
    public virtual async Task<TrixTask> UpdateAsync(
        string id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<TrixTask>(
            $"{BasePath}/{Uri.EscapeDataString(id)}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public new virtual async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Marks a task as done. The API has no dedicated complete endpoint, so this
    /// is a convenience wrapper over <see cref="UpdateAsync"/> with
    /// <see cref="TaskStatuses.Done"/>.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task.</returns>
    public virtual async Task<TrixTask> CompleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return await UpdateAsync(
            id, new UpdateTaskRequest { Status = TaskStatuses.Done }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists tasks. Either <see cref="ListTasksRequest.SpaceId"/> or
    /// <see cref="ListTasksRequest.AssigneeId"/> must be set.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A page of tasks.</returns>
    public virtual async Task<TaskList> ListAsync(
        ListTasksRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrEmpty(request.SpaceId) && string.IsNullOrEmpty(request.AssigneeId))
        {
            throw new ArgumentException(
                "ListAsync requires either SpaceId or AssigneeId.", nameof(request));
        }
        return await GetAsync<TaskList>(BasePath, BuildListQuery(request), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Iterates through all tasks matching the criteria, following offset pages.
    /// </summary>
    /// <param name="request">Filter parameters (SpaceId or AssigneeId required).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of tasks.</returns>
    public virtual async IAsyncEnumerable<TrixTask> ListAllAsync(
        ListTasksRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Limit ??= 100;
        request.Offset ??= 0;

        int pages = 0;
        const int MaxPages = 1000;
        while (true)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

            var page = await ListAsync(request, cancellationToken).ConfigureAwait(false);
            foreach (var task in page.Tasks)
            {
                yield return task;
            }

            if (!page.HasMore || page.Tasks.Count == 0)
            {
                break;
            }

            request.Offset += page.Tasks.Count;
        }
    }

    /// <summary>
    /// Creates a subtask under a parent task.
    /// </summary>
    /// <param name="parentId">The parent task ID.</param>
    /// <param name="request">The subtask creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created subtask.</returns>
    public virtual async Task<TrixTask> CreateSubtaskAsync(
        string parentId,
        CreateSubtaskRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(parentId);
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<TrixTask>(
            $"{BasePath}/{Uri.EscapeDataString(parentId)}/subtasks", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the subtasks of a task.
    /// </summary>
    /// <param name="parentId">The parent task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The subtasks.</returns>
    public virtual async Task<List<TrixTask>> GetSubtasksAsync(
        string parentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(parentId);
        var response = await GetAsync<SubtasksResponse>(
            $"{BasePath}/{Uri.EscapeDataString(parentId)}/subtasks",
            cancellationToken: cancellationToken).ConfigureAwait(false);
        return response.Subtasks;
    }

    private static Dictionary<string, string?> BuildListQuery(ListTasksRequest r)
    {
        var q = new Dictionary<string, string?>();
        if (r.SpaceId != null) q["space_id"] = r.SpaceId;
        if (r.AssigneeId != null) q["assignee_id"] = r.AssigneeId;
        if (r.AssigneeType != null) q["assignee_type"] = r.AssigneeType;
        if (r.Status != null) q["status"] = r.Status;
        if (r.Priority != null) q["priority"] = r.Priority.Value.ToString();
        if (r.ParentTaskId != null) q["parent_task_id"] = r.ParentTaskId;
        if (r.ProjectId != null) q["project_id"] = r.ProjectId;
        if (r.DueBefore != null) q["due_before"] = r.DueBefore.Value.ToString("o");
        if (r.DueAfter != null) q["due_after"] = r.DueAfter.Value.ToString("o");
        if (r.IncludeCompleted != null) q["include_completed"] = r.IncludeCompleted.Value ? "true" : "false";
        if (r.Limit != null) q["limit"] = r.Limit.Value.ToString();
        if (r.Offset != null) q["offset"] = r.Offset.Value.ToString();
        return q;
    }

    private sealed class SubtasksResponse
    {
        [JsonPropertyName("subtasks")]
        public List<TrixTask> Subtasks { get; set; } = new();
    }
}
