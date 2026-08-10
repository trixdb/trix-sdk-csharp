using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Job status.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum JobStatus
{
    [JsonPropertyName("waiting")]
    Waiting,
    [JsonPropertyName("active")]
    Active,
    [JsonPropertyName("completed")]
    Completed,
    [JsonPropertyName("failed")]
    Failed,
    [JsonPropertyName("delayed")]
    Delayed
}

/// <summary>
/// A background job.
/// </summary>
public class Job
{
    /// <summary>
    /// Unique identifier for the job.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Queue the job belongs to.
    /// </summary>
    [JsonPropertyName("queue")]
    public string Queue { get; set; } = string.Empty;

    /// <summary>
    /// Job name/type.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Job data/payload.
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// Current status.
    /// </summary>
    [JsonPropertyName("status")]
    public JobStatus Status { get; set; }

    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    [JsonPropertyName("progress")]
    public int? Progress { get; set; }

    /// <summary>
    /// Return value if completed.
    /// </summary>
    [JsonPropertyName("returnValue")]
    public object? ReturnValue { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    [JsonPropertyName("failedReason")]
    public string? FailedReason { get; set; }

    /// <summary>
    /// Number of attempts made.
    /// </summary>
    [JsonPropertyName("attempts")]
    public int Attempts { get; set; }

    /// <summary>
    /// When the job was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When processing started.
    /// </summary>
    [JsonPropertyName("processedAt")]
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// When the job completed.
    /// </summary>
    [JsonPropertyName("finishedAt")]
    public DateTime? FinishedAt { get; set; }
}

/// <summary>
/// Statistics for a job queue.
/// </summary>
public class QueueStats
{
    /// <summary>
    /// Queue name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Number of waiting jobs.
    /// </summary>
    [JsonPropertyName("waiting")]
    public int Waiting { get; set; }

    /// <summary>
    /// Number of active jobs.
    /// </summary>
    [JsonPropertyName("active")]
    public int Active { get; set; }

    /// <summary>
    /// Number of completed jobs.
    /// </summary>
    [JsonPropertyName("completed")]
    public int Completed { get; set; }

    /// <summary>
    /// Number of failed jobs.
    /// </summary>
    [JsonPropertyName("failed")]
    public int Failed { get; set; }

    /// <summary>
    /// Number of delayed jobs.
    /// </summary>
    [JsonPropertyName("delayed")]
    public int Delayed { get; set; }
}

/// <summary>
/// Overall job statistics.
/// </summary>
public class JobStats
{
    /// <summary>
    /// Statistics per queue.
    /// </summary>
    [JsonPropertyName("queues")]
    public List<QueueStats> Queues { get; set; } = new();
}

/// <summary>
/// Request parameters for listing jobs.
/// </summary>
public class ListJobsRequest
{
    /// <summary>
    /// Filter by queue name.
    /// </summary>
    [JsonPropertyName("queue")]
    public string? Queue { get; set; }

    /// <summary>
    /// Filter by status.
    /// </summary>
    [JsonPropertyName("status")]
    public JobStatus? Status { get; set; }

    /// <summary>
    /// Maximum number of results.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Offset for pagination.
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}

/// <summary>
/// Result of cleaning old jobs.
/// </summary>
public class CleanJobsResult
{
    /// <summary>
    /// Number of jobs removed.
    /// </summary>
    [JsonPropertyName("removed")]
    public int Removed { get; set; }
}
