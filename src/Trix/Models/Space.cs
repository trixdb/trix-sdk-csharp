using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents a space (workspace) in Trix.
/// </summary>
public class Space
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the space name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the space slug (URL-friendly identifier).</summary>
    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    /// <summary>Gets or sets the space description.</summary>
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
/// Parameters for creating a space.
/// </summary>
public class CreateSpaceRequest
{
    /// <summary>Gets or sets the space name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the space slug (URL-friendly identifier). Auto-generated if not provided.</summary>
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    /// <summary>Gets or sets the space description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Parameters for updating a space.
/// </summary>
public class UpdateSpaceRequest
{
    /// <summary>Gets or sets the space name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the space slug (URL-friendly identifier).</summary>
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    /// <summary>Gets or sets the space description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}
