using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Enrichment type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum EnrichmentType
{
    [JsonPropertyName("entities")]
    Entities,
    [JsonPropertyName("summary")]
    Summary,
    [JsonPropertyName("sentiment")]
    Sentiment,
    [JsonPropertyName("topics")]
    Topics,
    [JsonPropertyName("keywords")]
    Keywords,
    [JsonPropertyName("custom")]
    Custom
}

/// <summary>
/// Enrichment processing status.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum EnrichmentStatus
{
    [JsonPropertyName("pending")]
    Pending,
    [JsonPropertyName("processing")]
    Processing,
    [JsonPropertyName("completed")]
    Completed,
    [JsonPropertyName("failed")]
    Failed
}

/// <summary>
/// An enrichment applied to a memory.
/// </summary>
public class Enrichment
{
    /// <summary>
    /// Enrichment type.
    /// </summary>
    [JsonPropertyName("type")]
    public EnrichmentType Type { get; set; }

    /// <summary>
    /// Processing status.
    /// </summary>
    [JsonPropertyName("status")]
    public EnrichmentStatus Status { get; set; }

    /// <summary>
    /// Enrichment data/results.
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// When processing completed.
    /// </summary>
    [JsonPropertyName("processedAt")]
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// When the enrichment was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the enrichment was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request to trigger enrichments.
/// </summary>
public class TriggerEnrichmentRequest
{
    /// <summary>
    /// Types of enrichments to trigger.
    /// </summary>
    [JsonPropertyName("types")]
    public List<EnrichmentType>? Types { get; set; }

    /// <summary>
    /// Save results to the memory.
    /// </summary>
    [JsonPropertyName("save")]
    public bool? Save { get; set; }
}

/// <summary>
/// Result of triggering enrichments.
/// </summary>
public class TriggerEnrichmentResult
{
    /// <summary>
    /// Memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Triggered enrichment types.
    /// </summary>
    [JsonPropertyName("triggered")]
    public List<EnrichmentType> Triggered { get; set; } = new();

    /// <summary>
    /// Job IDs for tracking.
    /// </summary>
    [JsonPropertyName("jobIds")]
    public List<string>? JobIds { get; set; }
}
