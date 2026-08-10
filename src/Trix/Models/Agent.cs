using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// An agent session for conversation tracking.
/// </summary>
public class Session
{
    /// <summary>
    /// Unique identifier for the session.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Space ID the session belongs to.
    /// </summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Session name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// When the session started.
    /// </summary>
    [JsonPropertyName("startedAt")]
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// When the session ended.
    /// </summary>
    [JsonPropertyName("endedAt")]
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Number of memories in the session.
    /// </summary>
    [JsonPropertyName("memoryCount")]
    public int MemoryCount { get; set; }
}

/// <summary>
/// Request to create a session.
/// </summary>
public class CreateSessionRequest
{
    /// <summary>
    /// Session name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Space ID for the session.
    /// </summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// A memory within a session.
/// </summary>
public class SessionMemory
{
    /// <summary>
    /// Session memory ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Session ID.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Sequence number in the session.
    /// </summary>
    [JsonPropertyName("sequenceNumber")]
    public int SequenceNumber { get; set; }

    /// <summary>
    /// When the memory was added to the session.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request to add a memory to a session.
/// </summary>
public class AddSessionMemoryRequest
{
    /// <summary>
    /// Existing memory ID to add.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string? MemoryId { get; set; }

    /// <summary>
    /// Content for a new memory.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Memory type.
    /// </summary>
    [JsonPropertyName("type")]
    public MemoryType? Type { get; set; }

    /// <summary>
    /// Tags for the memory.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Session with its memory history.
/// </summary>
public class SessionHistory
{
    /// <summary>
    /// The session.
    /// </summary>
    [JsonPropertyName("session")]
    public Session Session { get; set; } = new();

    /// <summary>
    /// Memories in the session.
    /// </summary>
    [JsonPropertyName("memories")]
    public List<Memory> Memories { get; set; } = new();
}

/// <summary>
/// A core memory block.
/// </summary>
public class CoreMemoryBlock
{
    /// <summary>
    /// Block type (e.g., "user_info", "preferences").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Block content.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// When the block was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Core memory for an agent session.
/// </summary>
public class CoreMemory
{
    /// <summary>
    /// Core memory blocks.
    /// </summary>
    [JsonPropertyName("blocks")]
    public List<CoreMemoryBlock> Blocks { get; set; } = new();

    /// <summary>
    /// When the core memory was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Formatted core memory for prompt injection.
/// </summary>
public class CoreMemoryContext
{
    /// <summary>
    /// Formatted string representation.
    /// </summary>
    [JsonPropertyName("formatted")]
    public string Formatted { get; set; } = string.Empty;

    /// <summary>
    /// Individual blocks.
    /// </summary>
    [JsonPropertyName("blocks")]
    public List<CoreMemoryBlock> Blocks { get; set; } = new();
}

/// <summary>
/// Consolidation strategy.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ConsolidationStrategy
{
    [JsonPropertyName("similarity")]
    Similarity,
    [JsonPropertyName("temporal")]
    Temporal,
    [JsonPropertyName("importance")]
    Importance
}

/// <summary>
/// Request parameters for consolidation.
/// </summary>
public class ConsolidateRequest
{
    /// <summary>
    /// Session ID to consolidate.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    /// Space ID to consolidate.
    /// </summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Consolidation strategy.
    /// </summary>
    [JsonPropertyName("strategy")]
    public ConsolidationStrategy? Strategy { get; set; }

    /// <summary>
    /// Minimum similarity threshold.
    /// </summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>
    /// Maximum clusters to create.
    /// </summary>
    [JsonPropertyName("maxClusters")]
    public int? MaxClusters { get; set; }

    /// <summary>
    /// Run as dry run (don't make changes).
    /// </summary>
    [JsonPropertyName("dryRun")]
    public bool? DryRun { get; set; }
}

/// <summary>
/// Result of a consolidation operation.
/// </summary>
public class ConsolidationResult
{
    /// <summary>
    /// Job ID for tracking.
    /// </summary>
    [JsonPropertyName("jobId")]
    public string? JobId { get; set; }

    /// <summary>
    /// Number of memories consolidated.
    /// </summary>
    [JsonPropertyName("memoriesConsolidated")]
    public int MemoriesConsolidated { get; set; }

    /// <summary>
    /// Number of clusters created.
    /// </summary>
    [JsonPropertyName("clustersCreated")]
    public int ClustersCreated { get; set; }

    /// <summary>
    /// Number of relationships created.
    /// </summary>
    [JsonPropertyName("relationshipsCreated")]
    public int RelationshipsCreated { get; set; }
}

/// <summary>
/// Request to end a session.
/// </summary>
public class EndSessionRequest
{
    /// <summary>
    /// Consolidate memories after ending.
    /// </summary>
    [JsonPropertyName("consolidate")]
    public bool? Consolidate { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Result of ending a session.
/// </summary>
public class EndSessionResult
{
    /// <summary>
    /// The ended session.
    /// </summary>
    [JsonPropertyName("session")]
    public Session Session { get; set; } = new();

    /// <summary>
    /// Consolidation job ID if consolidation was requested.
    /// </summary>
    [JsonPropertyName("consolidationJobId")]
    public string? ConsolidationJobId { get; set; }
}

/// <summary>
/// Request for agent context.
/// </summary>
public class AgentContextRequest
{
    /// <summary>
    /// Session ID for context.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    /// Query to find relevant context.
    /// </summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>
    /// Maximum number of memories to return.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Include related memories.
    /// </summary>
    [JsonPropertyName("includeRelated")]
    public bool? IncludeRelated { get; set; }
}

/// <summary>
/// Context result for an agent.
/// </summary>
public class AgentContextResult
{
    /// <summary>
    /// Relevant memories.
    /// </summary>
    [JsonPropertyName("memories")]
    public List<Memory> Memories { get; set; } = new();

    /// <summary>
    /// Related relationships.
    /// </summary>
    [JsonPropertyName("relationships")]
    public List<Relationship>? Relationships { get; set; }
}

/// <summary>
/// Request parameters for listing sessions.
/// </summary>
public class ListSessionsRequest
{
    /// <summary>
    /// Maximum number of results.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Page number.
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    /// Filter by space ID.
    /// </summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    [JsonPropertyName("active")]
    public bool? Active { get; set; }
}

/// <summary>
/// ADR-112 P10 — Response from the explicit-trigger endpoints
/// (session_summary, mega_summary, scoped_fact). Returns 202 + job ID.
/// </summary>
public class TriggerJobResult
{
    /// <summary>Scope identifier (set on mega_summary / scoped_fact triggers).</summary>
    [JsonPropertyName("scope_id")]
    public string? ScopeId { get; set; }

    /// <summary>Session identifier (set on session_summary triggers).</summary>
    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    /// <summary>Scope type ('account' | 'space' | 'session' | 'window').</summary>
    [JsonPropertyName("scope_type")]
    public string? ScopeType { get; set; }

    /// <summary>True when the worker queue accepted the job.</summary>
    [JsonPropertyName("enqueued")]
    public bool Enqueued { get; set; }

    /// <summary>BullMQ job ID; null if the queue isn't decorated.</summary>
    [JsonPropertyName("job_id")]
    public string? JobId { get; set; }

    /// <summary>Pipeline preset name applied; null if none was supplied.</summary>
    [JsonPropertyName("pipeline")]
    public string? Pipeline { get; set; }
}

/// <summary>
/// ADR-109a — Response from GET /v1/pipeline-presets/_default and
/// POST /v1/pipeline-presets/:name/set-default.
/// </summary>
public class AccountDefaultPresetResult
{
    /// <summary>The default preset name (null when unset).</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
