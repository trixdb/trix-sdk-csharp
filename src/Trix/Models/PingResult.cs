using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Result of a health-check ping (ADR-143).
///
/// Returned by <see cref="TrixClient.PingAsync"/>. The server returns
/// {status, timestamp, uptime, version}; this envelope adds the
/// client-measured round-trip time so callers don't need a second
/// timing measurement.
/// </summary>
public class PingResult
{
    /// <summary>True when the server reports status == "ok".</summary>
    [JsonIgnore]
    public bool Ok { get; set; }

    /// <summary>Server version string, when reported.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>Client-measured round-trip time in milliseconds.</summary>
    [JsonIgnore]
    public long LatencyMs { get; set; }
}

/// <summary>
/// Raw server health-check response shape (internal — callers should
/// use <see cref="PingResult"/>).
/// </summary>
internal class HealthResponse
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("uptime")]
    public double? Uptime { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}
