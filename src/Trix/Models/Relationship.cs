using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents a relationship between two memories.
/// </summary>
public class Relationship
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the source memory ID.</summary>
    [JsonPropertyName("source_id")]
    public required string SourceId { get; set; }

    /// <summary>Gets or sets the target memory ID.</summary>
    [JsonPropertyName("target_id")]
    public required string TargetId { get; set; }

    /// <summary>Gets or sets the relationship type.</summary>
    [JsonPropertyName("relationship_type")]
    public required string RelationshipType { get; set; }

    /// <summary>Gets or sets the relationship weight (0-1). ADR-145: wire field name is `weight`.</summary>
    [JsonPropertyName("weight")]
    public double Weight { get; set; } = 1.0;

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Parameters for creating a relationship (ADR-145 wire contract).
/// SourceId is omitted from the body — it's the URL path segment.
/// </summary>
public class CreateRelationshipRequest
{
    /// <summary>Gets or sets the target memory ID.</summary>
    [JsonPropertyName("target_id")]
    public required string TargetId { get; set; }

    /// <summary>Gets or sets the relationship type.</summary>
    [JsonPropertyName("relationship_type")]
    public required string RelationshipType { get; set; }

    /// <summary>Gets or sets the relationship weight.</summary>
    [JsonPropertyName("weight")]
    public double Weight { get; set; } = 1.0;

    /// <summary>Gets or sets the relationship description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets whether the relationship is bidirectional.</summary>
    [JsonPropertyName("bidirectional")]
    public bool? Bidirectional { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Parameters for updating a relationship.
/// </summary>
public class UpdateRelationshipRequest
{
    /// <summary>Gets or sets the relationship type.</summary>
    [JsonPropertyName("relationship_type")]
    public string? RelationshipType { get; set; }

    /// <summary>Gets or sets the relationship weight.</summary>
    [JsonPropertyName("weight")]
    public double? Weight { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Common relationship types.
/// </summary>
public static class RelationshipTypes
{
    public const string RelatedTo = "related_to";
    public const string Supports = "supports";
    public const string Contradicts = "contradicts";
    public const string References = "references";
    public const string Extends = "extends";
    public const string DependsOn = "depends_on";
    public const string Contains = "contains";
    public const string PartOf = "part_of";
    public const string Causes = "causes";
    public const string CausedBy = "caused_by";
}
