using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Result of <see cref="Resources.AgentResource.ResolvePipelineAsync"/>
/// (ADR-109a observability, tick 79).
///
/// Returns what the 3-tier resolver (caller > space > account > none) would
/// apply right now for a given (spaceId, pipeline) pair. All three fields
/// may be null simultaneously when no preset applies at any tier.
/// </summary>
public class PipelineResolution
{
    /// <summary>Resolved preset name, or null when no preset applies.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Which tier won. One of "caller", "space", "account", or null.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Full preset spec as a raw JsonElement (null when no preset applies).
    /// Callers can navigate the spec via the JsonElement API.
    /// </summary>
    [JsonPropertyName("preset")]
    public JsonElement? Preset { get; set; }
}
