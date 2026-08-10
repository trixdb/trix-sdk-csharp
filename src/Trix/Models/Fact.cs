using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Type of node in a fact (subject or object).
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum FactNodeType
{
    /// <summary>An entity reference.</summary>
    [JsonPropertyName("entity")] Entity,

    /// <summary>Free text.</summary>
    [JsonPropertyName("text")] Text,

    /// <summary>A memory reference.</summary>
    [JsonPropertyName("memory")] Memory
}

/// <summary>
/// Source of a fact.
/// </summary>
public class FactSource
{
    /// <summary>Gets or sets the source memory ID.</summary>
    [JsonPropertyName("memoryId")]
    public string? MemoryId { get; set; }

    /// <summary>Gets or sets the session ID.</summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>Gets or sets how the fact was created.</summary>
    [JsonPropertyName("method")]
    public string? Method { get; set; }
}

/// <summary>
/// Represents a knowledge graph fact (Subject-Predicate-Object triple).
/// </summary>
public class Fact
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the subject of the fact.</summary>
    [JsonPropertyName("subject")]
    public required string Subject { get; set; }

    /// <summary>Gets or sets the predicate (relationship).</summary>
    [JsonPropertyName("predicate")]
    public required string Predicate { get; set; }

    /// <summary>Gets or sets the object of the fact.</summary>
    [JsonPropertyName("object")]
    public required string Object { get; set; }

    /// <summary>Gets or sets the subject type.</summary>
    [JsonPropertyName("subjectType")]
    public FactNodeType? SubjectType { get; set; }

    /// <summary>Gets or sets the object type.</summary>
    [JsonPropertyName("objectType")]
    public FactNodeType? ObjectType { get; set; }

    /// <summary>Gets or sets the confidence score (0-1).</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; } = 1.0;

    /// <summary>Gets or sets the source of the fact.</summary>
    [JsonPropertyName("source")]
    public FactSource? Source { get; set; }

    /// <summary>Gets or sets when the fact became valid.</summary>
    [JsonPropertyName("validFrom")]
    public DateTimeOffset? ValidFrom { get; set; }

    /// <summary>Gets or sets when the fact stopped being valid.</summary>
    [JsonPropertyName("validTo")]
    public DateTimeOffset? ValidTo { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Parameters for creating a fact.
/// </summary>
public class CreateFactRequest
{
    /// <summary>Gets or sets the subject.</summary>
    [JsonPropertyName("subject")]
    public required string Subject { get; set; }

    /// <summary>Gets or sets the predicate.</summary>
    [JsonPropertyName("predicate")]
    public required string Predicate { get; set; }

    /// <summary>Gets or sets the object.</summary>
    [JsonPropertyName("object")]
    public required string Object { get; set; }

    /// <summary>Gets or sets the subject type.</summary>
    [JsonPropertyName("subjectType")]
    public FactNodeType? SubjectType { get; set; }

    /// <summary>Gets or sets the object type.</summary>
    [JsonPropertyName("objectType")]
    public FactNodeType? ObjectType { get; set; }

    /// <summary>Gets or sets the confidence score.</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; } = 1.0;

    /// <summary>Gets or sets the source.</summary>
    [JsonPropertyName("source")]
    public FactSource? Source { get; set; }

    /// <summary>Gets or sets the valid from date.</summary>
    [JsonPropertyName("validFrom")]
    public DateTimeOffset? ValidFrom { get; set; }

    /// <summary>Gets or sets the valid to date.</summary>
    [JsonPropertyName("validTo")]
    public DateTimeOffset? ValidTo { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }
}

/// <summary>
/// A fact with a relevance score.
/// </summary>
public class ScoredFact : Fact
{
    /// <summary>Gets or sets the relevance score.</summary>
    [JsonPropertyName("score")]
    public double Score { get; set; }
}

/// <summary>
/// Parameters for updating a fact.
/// </summary>
public class UpdateFactRequest
{
    /// <summary>Gets or sets the subject.</summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    /// <summary>Gets or sets the predicate.</summary>
    [JsonPropertyName("predicate")]
    public string? Predicate { get; set; }

    /// <summary>Gets or sets the object.</summary>
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    /// <summary>Gets or sets the subject type.</summary>
    [JsonPropertyName("subjectType")]
    public FactNodeType? SubjectType { get; set; }

    /// <summary>Gets or sets the object type.</summary>
    [JsonPropertyName("objectType")]
    public FactNodeType? ObjectType { get; set; }

    /// <summary>Gets or sets the confidence score.</summary>
    [JsonPropertyName("confidence")]
    public double? Confidence { get; set; }

    /// <summary>Gets or sets the valid from date.</summary>
    [JsonPropertyName("validFrom")]
    public DateTimeOffset? ValidFrom { get; set; }

    /// <summary>Gets or sets the valid to date.</summary>
    [JsonPropertyName("validTo")]
    public DateTimeOffset? ValidTo { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// An extracted fact from a memory.
/// </summary>
public class ExtractedFact
{
    /// <summary>Gets or sets the subject.</summary>
    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>Gets or sets the predicate.</summary>
    [JsonPropertyName("predicate")]
    public string Predicate { get; set; } = string.Empty;

    /// <summary>Gets or sets the object.</summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    /// <summary>Gets or sets the confidence.</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }
}

/// <summary>
/// Result of fact extraction.
/// </summary>
public class FactExtractionResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>Gets or sets the extracted facts.</summary>
    [JsonPropertyName("facts")]
    public List<ExtractedFact> Facts { get; set; } = new();

    /// <summary>Gets or sets the saved fact IDs.</summary>
    [JsonPropertyName("savedIds")]
    public List<string>? SavedIds { get; set; }
}

/// <summary>
/// Result of fact verification.
/// </summary>
public class FactVerificationResult
{
    /// <summary>Gets or sets the fact ID.</summary>
    [JsonPropertyName("factId")]
    public string FactId { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the fact was verified.</summary>
    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    /// <summary>Gets or sets the verification confidence.</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    /// <summary>Gets or sets supporting memory IDs.</summary>
    [JsonPropertyName("supportingMemories")]
    public List<string>? SupportingMemories { get; set; }

    /// <summary>Gets or sets contradicting memory IDs.</summary>
    [JsonPropertyName("contradictingMemories")]
    public List<string>? ContradictingMemories { get; set; }
}
