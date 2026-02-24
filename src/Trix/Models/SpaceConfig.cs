using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// A configuration category with defaults, overrides, and effective values.
/// </summary>
public class SpaceConfigCategory
{
    /// <summary>Gets or sets the system-level defaults for this category.</summary>
    [JsonPropertyName("defaults")]
    public Dictionary<string, object?> Defaults { get; set; } = new();

    /// <summary>Gets or sets the space-specific overrides set by the user.</summary>
    [JsonPropertyName("overrides")]
    public Dictionary<string, object?> Overrides { get; set; } = new();

    /// <summary>Gets or sets the computed values (defaults merged with overrides).</summary>
    [JsonPropertyName("effective")]
    public Dictionary<string, object?> Effective { get; set; } = new();
}

/// <summary>
/// Full space configuration response with all categories and version.
/// </summary>
public class SpaceConfigResponse
{
    /// <summary>Gets or sets the configuration categories.</summary>
    [JsonPropertyName("config")]
    public Dictionary<string, SpaceConfigCategory> Config { get; set; } = new();

    /// <summary>Gets or sets the optimistic concurrency version number.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }
}

/// <summary>
/// Partial configuration patch using JSON Merge Patch semantics.
/// Only include the categories and keys you want to change.
/// Set a key to null to remove an override and revert to the default.
/// </summary>
public class SpaceConfigPatch
{
    /// <summary>Gets or sets memory configuration overrides.</summary>
    [JsonPropertyName("memory")]
    public Dictionary<string, object?>? Memory { get; set; }

    /// <summary>Gets or sets LLM configuration overrides.</summary>
    [JsonPropertyName("llm")]
    public Dictionary<string, object?>? Llm { get; set; }

    /// <summary>Gets or sets retrieval configuration overrides.</summary>
    [JsonPropertyName("retrieval")]
    public Dictionary<string, object?>? Retrieval { get; set; }

    /// <summary>Gets or sets privacy configuration overrides.</summary>
    [JsonPropertyName("privacy")]
    public Dictionary<string, object?>? Privacy { get; set; }
}

/// <summary>
/// Result of a dry-run configuration validation.
/// </summary>
public class SpaceConfigValidation
{
    /// <summary>Gets or sets whether the patch is valid.</summary>
    [JsonPropertyName("valid")]
    public bool Valid { get; set; }

    /// <summary>Gets or sets the list of validation error messages.</summary>
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    /// <summary>Gets or sets the list of affected category names.</summary>
    [JsonPropertyName("categories")]
    public List<string> Categories { get; set; } = new();
}

/// <summary>
/// A single audit trail event for a configuration change.
/// </summary>
public class SpaceConfigAuditEvent
{
    /// <summary>Gets or sets the unique event ID.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the space ID this event belongs to.</summary>
    [JsonPropertyName("space_id")]
    public string SpaceId { get; set; } = string.Empty;

    /// <summary>Gets or sets the configuration category that was changed.</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>Gets or sets the user who made the change (null for system changes).</summary>
    [JsonPropertyName("actor_user_id")]
    public string? ActorUserId { get; set; }

    /// <summary>Gets or sets the API key that made the change (null for user/system changes).</summary>
    [JsonPropertyName("actor_api_key_id")]
    public string? ActorApiKeyId { get; set; }

    /// <summary>Gets or sets the source of the change (e.g., 'api', 'system').</summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>Gets or sets the patch that was applied.</summary>
    [JsonPropertyName("patch")]
    public Dictionary<string, object?> Patch { get; set; } = new();

    /// <summary>Gets or sets the previous configuration value.</summary>
    [JsonPropertyName("previous_value")]
    public Dictionary<string, object?> PreviousValue { get; set; } = new();

    /// <summary>Gets or sets the ISO 8601 timestamp of when the change occurred.</summary>
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// Paginated audit trail response.
/// </summary>
public class SpaceConfigAuditResponse
{
    /// <summary>Gets or sets the list of audit events.</summary>
    [JsonPropertyName("events")]
    public List<SpaceConfigAuditEvent> Events { get; set; } = new();

    /// <summary>Gets or sets the total number of events.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>Gets or sets the page size.</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>Gets or sets the page offset.</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}
