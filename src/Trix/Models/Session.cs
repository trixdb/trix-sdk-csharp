using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents the type of a session.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SessionType
{
    /// <summary>Conversation session.</summary>
    [JsonPropertyName("conversation")] Conversation,

    /// <summary>Project session.</summary>
    [JsonPropertyName("project")] Project,

    /// <summary>Task session.</summary>
    [JsonPropertyName("task")] Task,

    /// <summary>Temporary session.</summary>
    [JsonPropertyName("temporary")] Temporary
}

/// <summary>
/// Represents the status of a session.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SessionStatus
{
    /// <summary>Session is active.</summary>
    [JsonPropertyName("active")] Active,

    /// <summary>Session is paused.</summary>
    [JsonPropertyName("paused")] Paused,

    /// <summary>Session is completed.</summary>
    [JsonPropertyName("completed")] Completed,

    /// <summary>Session is archived.</summary>
    [JsonPropertyName("archived")] Archived
}

/// <summary>
/// Represents the retention policy for a session.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RetentionPolicy
{
    /// <summary>Session is kept permanently.</summary>
    [JsonPropertyName("permanent")] Permanent,

    /// <summary>Session is auto-deleted after retention period.</summary>
    [JsonPropertyName("autoDelete")] AutoDelete,

    /// <summary>Session is deleted on completion.</summary>
    [JsonPropertyName("onCompletion")] OnCompletion,

    /// <summary>Temporary session.</summary>
    [JsonPropertyName("temporary")] Temporary
}

/// <summary>
/// Represents a CLI session in Trix.
/// </summary>
public class CliSession
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the account ID.</summary>
    [JsonPropertyName("accountId")]
    public required string AccountId { get; set; }

    /// <summary>Gets or sets the user ID.</summary>
    [JsonPropertyName("userId")]
    public required string UserId { get; set; }

    /// <summary>Gets or sets the creator ID.</summary>
    [JsonPropertyName("createdBy")]
    public required string CreatedBy { get; set; }

    /// <summary>Gets or sets the session name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the session description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the session type.</summary>
    [JsonPropertyName("type")]
    public SessionType Type { get; set; } = SessionType.Conversation;

    /// <summary>Gets or sets the session status.</summary>
    [JsonPropertyName("status")]
    public SessionStatus Status { get; set; } = SessionStatus.Active;

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the origin type.</summary>
    [JsonPropertyName("originType")]
    public string? OriginType { get; set; }

    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>Gets or sets the retention policy.</summary>
    [JsonPropertyName("retentionPolicy")]
    public RetentionPolicy RetentionPolicy { get; set; } = RetentionPolicy.Permanent;

    /// <summary>Gets or sets the retention days.</summary>
    [JsonPropertyName("retentionDays")]
    public int? RetentionDays { get; set; }

    /// <summary>Gets or sets the session summary.</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>Gets or sets whether the session is private.</summary>
    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    /// <summary>Gets or sets the message count.</summary>
    [JsonPropertyName("messageCount")]
    public int MessageCount { get; set; }

    /// <summary>Gets or sets the memory count.</summary>
    [JsonPropertyName("memoryCount")]
    public int MemoryCount { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Gets or sets the last active timestamp.</summary>
    [JsonPropertyName("lastActiveAt")]
    public DateTimeOffset LastActiveAt { get; set; }

    /// <summary>Gets or sets the paused timestamp.</summary>
    [JsonPropertyName("pausedAt")]
    public DateTimeOffset? PausedAt { get; set; }

    /// <summary>Gets or sets the completed timestamp.</summary>
    [JsonPropertyName("completedAt")]
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>Gets or sets the archived timestamp.</summary>
    [JsonPropertyName("archivedAt")]
    public DateTimeOffset? ArchivedAt { get; set; }
}

/// <summary>
/// Parameters for creating a CLI session.
/// </summary>
public class CreateCliSessionRequest
{
    /// <summary>Gets or sets the session name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the session description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the session type.</summary>
    [JsonPropertyName("type")]
    public SessionType? Type { get; set; }

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the origin type.</summary>
    [JsonPropertyName("originType")]
    public string? OriginType { get; set; }

    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>Gets or sets the retention policy.</summary>
    [JsonPropertyName("retentionPolicy")]
    public RetentionPolicy? RetentionPolicy { get; set; }

    /// <summary>Gets or sets the retention days.</summary>
    [JsonPropertyName("retentionDays")]
    public int? RetentionDays { get; set; }

    /// <summary>Gets or sets whether the session is private.</summary>
    [JsonPropertyName("isPrivate")]
    public bool? IsPrivate { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Parameters for updating a CLI session.
/// </summary>
public class UpdateCliSessionRequest
{
    /// <summary>Gets or sets the session name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the session description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>Gets or sets whether the session is private.</summary>
    [JsonPropertyName("isPrivate")]
    public bool? IsPrivate { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Parameters for completing a CLI session.
/// </summary>
public class CompleteCliSessionRequest
{
    /// <summary>Gets or sets the optional session summary.</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
}

/// <summary>
/// Parameters for listing CLI sessions.
/// </summary>
public class ListCliSessionsParams
{
    /// <summary>Filter by session status.</summary>
    public SessionStatus? Status { get; set; }

    /// <summary>Filter by session type.</summary>
    public SessionType? Type { get; set; }

    /// <summary>Filter by space ID.</summary>
    public string? SpaceId { get; set; }

    /// <summary>Filter by tags.</summary>
    public List<string>? Tags { get; set; }

    /// <summary>Search query string.</summary>
    public string? Search { get; set; }

    /// <summary>Maximum number of results to return.</summary>
    public int? Limit { get; set; }

    /// <summary>Page number for pagination.</summary>
    public int? Page { get; set; }

    /// <summary>Sort field.</summary>
    public string? SortBy { get; set; }

    /// <summary>Sort order (asc or desc).</summary>
    public string? SortOrder { get; set; }
}

/// <summary>
/// CLI session statistics.
/// </summary>
public class CliSessionStats
{
    /// <summary>Gets or sets the total number of sessions.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>Gets or sets the number of active sessions.</summary>
    [JsonPropertyName("active")]
    public int Active { get; set; }

    /// <summary>Gets or sets the number of paused sessions.</summary>
    [JsonPropertyName("paused")]
    public int Paused { get; set; }

    /// <summary>Gets or sets the number of completed sessions.</summary>
    [JsonPropertyName("completed")]
    public int Completed { get; set; }

    /// <summary>Gets or sets the number of archived sessions.</summary>
    [JsonPropertyName("archived")]
    public int Archived { get; set; }

    /// <summary>Gets or sets the count by type.</summary>
    [JsonPropertyName("byType")]
    public Dictionary<string, int>? ByType { get; set; }

    /// <summary>Gets or sets the count by space.</summary>
    [JsonPropertyName("bySpace")]
    public Dictionary<string, int>? BySpace { get; set; }

    /// <summary>Gets or sets the total message count.</summary>
    [JsonPropertyName("totalMessages")]
    public int? TotalMessages { get; set; }

    /// <summary>Gets or sets the total memory count.</summary>
    [JsonPropertyName("totalMemories")]
    public int? TotalMemories { get; set; }

    /// <summary>Gets or sets the average session duration in seconds.</summary>
    [JsonPropertyName("avgDuration")]
    public double? AvgDuration { get; set; }
}
