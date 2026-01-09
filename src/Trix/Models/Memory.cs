using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents the type of memory content.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MemoryType
{
    /// <summary>Plain text content.</summary>
    [JsonPropertyName("text")] Text,

    /// <summary>Markdown formatted content.</summary>
    [JsonPropertyName("markdown")] Markdown,

    /// <summary>URL content.</summary>
    [JsonPropertyName("url")] Url,

    /// <summary>Audio content.</summary>
    [JsonPropertyName("audio")] Audio,

    /// <summary>Image content.</summary>
    [JsonPropertyName("image")] Image
}

/// <summary>
/// Represents the protection level for a memory.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProtectionLevel
{
    /// <summary>No protection - memory can be modified or deleted freely.</summary>
    [JsonPropertyName("none")] None,

    /// <summary>Soft protection - requires confirmation for modifications.</summary>
    [JsonPropertyName("soft")] Soft,

    /// <summary>Hard protection - memory cannot be modified or deleted.</summary>
    [JsonPropertyName("hard")] Hard
}

/// <summary>
/// Represents the transcript status of audio memories.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TranscriptStatus
{
    /// <summary>Transcription is pending.</summary>
    [JsonPropertyName("pending")] Pending,

    /// <summary>Transcription is in progress.</summary>
    [JsonPropertyName("processing")] Processing,

    /// <summary>Transcription completed successfully.</summary>
    [JsonPropertyName("completed")] Completed,

    /// <summary>Transcription failed.</summary>
    [JsonPropertyName("failed")] Failed
}

/// <summary>
/// Represents a memory in Trix.
/// </summary>
public class Memory
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the space this memory belongs to.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the content type.</summary>
    [JsonPropertyName("type")]
    public MemoryType Type { get; set; } = MemoryType.Text;

    /// <summary>Gets or sets the content.</summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    /// <summary>Gets or sets the embedding vector.</summary>
    [JsonPropertyName("embedding")]
    public float[]? Embedding { get; set; }

    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Gets or sets the transcript status for audio memories.</summary>
    [JsonPropertyName("transcriptStatus")]
    public TranscriptStatus? TranscriptStatus { get; set; }

    /// <summary>Gets or sets whether this memory is pinned.</summary>
    [JsonPropertyName("isPinned")]
    public bool IsPinned { get; set; }

    /// <summary>Gets or sets the protection level for this memory.</summary>
    [JsonPropertyName("protectionLevel")]
    public ProtectionLevel ProtectionLevel { get; set; } = ProtectionLevel.None;

    /// <summary>Gets or sets whether this memory is soft deleted.</summary>
    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }

    /// <summary>Gets or sets the deletion timestamp (for soft deleted memories).</summary>
    [JsonPropertyName("deletedAt")]
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>Gets or sets the quality score for this memory (0-1).</summary>
    [JsonPropertyName("qualityScore")]
    public double? QualityScore { get; set; }
}

/// <summary>
/// Parameters for creating a new memory.
/// </summary>
public class CreateMemoryRequest
{
    /// <summary>Gets or sets the content.</summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    /// <summary>Gets or sets the content type.</summary>
    [JsonPropertyName("type")]
    public MemoryType Type { get; set; } = MemoryType.Text;

    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets a pre-computed embedding.</summary>
    [JsonPropertyName("embedding")]
    public float[]? Embedding { get; set; }

    /// <summary>Gets or sets whether the memory should be pinned on creation.</summary>
    [JsonPropertyName("isPinned")]
    public bool IsPinned { get; set; }

    /// <summary>Gets or sets the protection level for the memory.</summary>
    [JsonPropertyName("protectionLevel")]
    public ProtectionLevel ProtectionLevel { get; set; } = ProtectionLevel.None;
}

/// <summary>
/// Parameters for updating an existing memory.
/// </summary>
public class UpdateMemoryRequest
{
    /// <summary>Gets or sets the content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the embedding.</summary>
    [JsonPropertyName("embedding")]
    public float[]? Embedding { get; set; }
}

/// <summary>
/// Parameters for listing memories.
/// </summary>
public class ListMemoriesRequest
{
    /// <summary>Search query string.</summary>
    public string? Q { get; set; }

    /// <summary>Search mode (semantic, keyword, or hybrid).</summary>
    public SearchMode? Mode { get; set; }

    /// <summary>Maximum number of results to return.</summary>
    public int? Limit { get; set; }

    /// <summary>Page number for pagination.</summary>
    public int? Page { get; set; }

    /// <summary>Offset for pagination.</summary>
    public int? Offset { get; set; }

    /// <summary>Filter by tags.</summary>
    public List<string>? Tags { get; set; }

    /// <summary>Filter by memory type.</summary>
    public MemoryType? Type { get; set; }

    /// <summary>Filter by space ID.</summary>
    public string? SpaceId { get; set; }

    /// <summary>Sort field.</summary>
    public string? SortBy { get; set; }

    /// <summary>Sort order (asc or desc).</summary>
    public string? SortOrder { get; set; }

    /// <summary>Filter by pinned status (true for pinned only, false for unpinned only).</summary>
    public bool? Pinned { get; set; }

    /// <summary>Filter by protection status (true for protected memories only).</summary>
    public bool? Protected { get; set; }

    /// <summary>Filter by minimum quality score (0-1).</summary>
    public double? MinQuality { get; set; }

    /// <summary>Include soft-deleted memories in results.</summary>
    public bool? IncludeDeleted { get; set; }
}

/// <summary>
/// Search mode for memory queries.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SearchMode
{
    /// <summary>Semantic search using embeddings.</summary>
    [JsonPropertyName("semantic")] Semantic,

    /// <summary>Keyword-based search.</summary>
    [JsonPropertyName("keyword")] Keyword,

    /// <summary>Hybrid search combining semantic and keyword.</summary>
    [JsonPropertyName("hybrid")] Hybrid
}

/// <summary>
/// Memory system configuration.
/// </summary>
public class MemoryConfig
{
    /// <summary>Gets or sets the maximum content length.</summary>
    [JsonPropertyName("maxContentLength")]
    public int MaxContentLength { get; set; }

    /// <summary>Gets or sets the supported memory types.</summary>
    [JsonPropertyName("supportedTypes")]
    public List<MemoryType>? SupportedTypes { get; set; }

    /// <summary>Gets or sets the maximum tags per memory.</summary>
    [JsonPropertyName("maxTagsPerMemory")]
    public int MaxTagsPerMemory { get; set; }

    /// <summary>Gets or sets the maximum audio duration in seconds.</summary>
    [JsonPropertyName("maxAudioDuration")]
    public int MaxAudioDuration { get; set; }
}

/// <summary>
/// Parameters for getting memory statistics.
/// </summary>
public class GetMemoryStatsRequest
{
    /// <summary>Gets or sets the space ID filter.</summary>
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the created after filter.</summary>
    public DateTimeOffset? CreatedAfter { get; set; }

    /// <summary>Gets or sets the created before filter.</summary>
    public DateTimeOffset? CreatedBefore { get; set; }

    /// <summary>Gets or sets whether to include type distribution.</summary>
    public bool? IncludeTypeDistribution { get; set; }

    /// <summary>Gets or sets whether to include tag distribution.</summary>
    public bool? IncludeTagDistribution { get; set; }

    /// <summary>Gets or sets whether to include timeline data.</summary>
    public bool? IncludeTimeline { get; set; }

    /// <summary>Gets or sets the timeline granularity.</summary>
    public string? TimelineGranularity { get; set; }
}

/// <summary>
/// Memory statistics.
/// </summary>
public class MemoryStats
{
    /// <summary>Gets or sets the total count.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>Gets or sets the count by type.</summary>
    [JsonPropertyName("byType")]
    public Dictionary<string, int>? ByType { get; set; }

    /// <summary>Gets or sets the count by tag.</summary>
    [JsonPropertyName("byTag")]
    public Dictionary<string, int>? ByTag { get; set; }

    /// <summary>Gets or sets the timeline data.</summary>
    [JsonPropertyName("timeline")]
    public List<TimelineEntry>? Timeline { get; set; }

    /// <summary>Gets or sets the average content length.</summary>
    [JsonPropertyName("avgContentLength")]
    public double? AvgContentLength { get; set; }

    /// <summary>Gets or sets the total size in bytes.</summary>
    [JsonPropertyName("totalSize")]
    public long? TotalSize { get; set; }
}

/// <summary>
/// Timeline entry for statistics.
/// </summary>
public class TimelineEntry
{
    /// <summary>Gets or sets the period.</summary>
    [JsonPropertyName("period")]
    public string Period { get; set; } = string.Empty;

    /// <summary>Gets or sets the count.</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
}
