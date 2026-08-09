using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Result of an MQL query. Captures either result shape the <c>?mql=</c> endpoint
/// returns: a normal page (<see cref="Data"/> + <see cref="Pagination"/>) or a
/// <c>group by …</c> aggregation (<see cref="Aggregate"/> + <see cref="GroupBy"/>
/// + <see cref="Metrics"/>). Check <see cref="IsAggregate"/> to disambiguate.
/// </summary>
public class MqlResult
{
    /// <summary>Gets or sets the matching memories (normal query).</summary>
    [JsonPropertyName("data")]
    public List<Memory>? Data { get; set; }

    /// <summary>Gets or sets the pagination metadata (normal query).</summary>
    [JsonPropertyName("pagination")]
    public PaginationMetadata? Pagination { get; set; }

    /// <summary>Gets or sets the aggregation buckets (<c>group by</c> query).</summary>
    [JsonPropertyName("aggregate")]
    public List<Dictionary<string, JsonElement>>? Aggregate { get; set; }

    /// <summary>Gets or sets the field the aggregation grouped by, if any.</summary>
    [JsonPropertyName("group_by")]
    public string? GroupBy { get; set; }

    /// <summary>Gets or sets the metric aliases returned per bucket.</summary>
    [JsonPropertyName("metrics")]
    public List<string>? Metrics { get; set; }

    /// <summary>Gets a value indicating whether this is a <c>group by</c> aggregation.</summary>
    [JsonIgnore]
    public bool IsAggregate => Aggregate != null;
}
