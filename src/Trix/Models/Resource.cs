using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents a resource (project, topic, etc.) in Trix.
/// </summary>
public class Resource
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the resource name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the resource type.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "project";

    /// <summary>Gets or sets the resource description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Parameters for creating a resource.
/// </summary>
public class CreateResourceRequest
{
    /// <summary>Gets or sets the resource name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the resource type.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets the resource description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Parameters for updating a resource.
/// </summary>
public class UpdateResourceRequest
{
    /// <summary>Gets or sets the resource name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the resource type.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets the resource description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Relationship types for memory-resource associations.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ResourceRelationshipType
{
    /// <summary>Primary resource relationship.</summary>
    [JsonPropertyName("primary")]
    Primary,

    /// <summary>Related resource relationship.</summary>
    [JsonPropertyName("related")]
    Related,

    /// <summary>Mentioned resource relationship.</summary>
    [JsonPropertyName("mentioned")]
    Mentioned,

    /// <summary>Derived resource relationship.</summary>
    [JsonPropertyName("derived")]
    Derived
}

/// <summary>
/// Parameters for linking a resource to a memory.
/// </summary>
public class LinkResourceRequest
{
    /// <summary>Gets or sets the resource ID to link.</summary>
    [JsonPropertyName("resource_id")]
    public required string ResourceId { get; set; }

    /// <summary>Gets or sets the relationship type.</summary>
    [JsonPropertyName("relationship_type")]
    public ResourceRelationshipType? RelationshipType { get; set; }
}

/// <summary>
/// Result of linking a resource to a memory.
/// </summary>
public class LinkResourceResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memory_id")]
    public required string MemoryId { get; set; }

    /// <summary>Gets or sets the resource ID.</summary>
    [JsonPropertyName("resource_id")]
    public required string ResourceId { get; set; }

    /// <summary>Gets or sets the relationship type.</summary>
    [JsonPropertyName("relationship_type")]
    public ResourceRelationshipType RelationshipType { get; set; }

    /// <summary>Gets or sets whether the link was successful.</summary>
    [JsonPropertyName("linked")]
    public bool Linked { get; set; }
}

/// <summary>
/// Result of unlinking a resource from a memory.
/// </summary>
public class UnlinkResourceResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memory_id")]
    public required string MemoryId { get; set; }

    /// <summary>Gets or sets the resource ID.</summary>
    [JsonPropertyName("resource_id")]
    public required string ResourceId { get; set; }

    /// <summary>Gets or sets whether the unlink was successful.</summary>
    [JsonPropertyName("unlinked")]
    public bool Unlinked { get; set; }
}

/// <summary>
/// A resource linked to a memory with relationship metadata.
/// </summary>
public class LinkedResource
{
    /// <summary>Gets or sets the resource ID.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the resource name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the resource type.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets the relationship type.</summary>
    [JsonPropertyName("relationship_type")]
    public ResourceRelationshipType RelationshipType { get; set; }

    /// <summary>Gets or sets when the resource was linked.</summary>
    [JsonPropertyName("linked_at")]
    public DateTimeOffset LinkedAt { get; set; }

    /// <summary>Gets or sets additional properties.</summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

/// <summary>
/// Response containing resources linked to a memory.
/// </summary>
public class MemoryResourcesResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memory_id")]
    public required string MemoryId { get; set; }

    /// <summary>Gets or sets the list of linked resources.</summary>
    [JsonPropertyName("data")]
    public List<LinkedResource> Data { get; set; } = new();
}

/// <summary>
/// A memory linked to a resource with relationship metadata.
/// </summary>
public class LinkedMemory
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the memory content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>Gets or sets the relationship type.</summary>
    [JsonPropertyName("relationship_type")]
    public ResourceRelationshipType RelationshipType { get; set; }

    /// <summary>Gets or sets when the memory was linked.</summary>
    [JsonPropertyName("linked_at")]
    public DateTimeOffset LinkedAt { get; set; }

    /// <summary>Gets or sets additional properties.</summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

/// <summary>
/// Pagination metadata for resource memories result.
/// </summary>
public class ResourceMemoriesPagination
{
    /// <summary>Gets or sets the total count.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>Gets or sets the page limit.</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>Gets or sets the offset.</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>Gets or sets whether there are more results.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}

/// <summary>
/// Response containing memories linked to a resource.
/// </summary>
public class ResourceMemoriesResult
{
    /// <summary>Gets or sets the resource ID.</summary>
    [JsonPropertyName("resource_id")]
    public required string ResourceId { get; set; }

    /// <summary>Gets or sets the list of linked memories.</summary>
    [JsonPropertyName("data")]
    public List<LinkedMemory> Data { get; set; } = new();

    /// <summary>Gets or sets pagination metadata.</summary>
    [JsonPropertyName("pagination")]
    public ResourceMemoriesPagination? Pagination { get; set; }
}
