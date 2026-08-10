using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Highlight extraction method.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ExtractionMethod
{
    [JsonPropertyName("ai")]
    AI,
    [JsonPropertyName("keyword")]
    Keyword,
    [JsonPropertyName("tfidf")]
    TfIdf
}

/// <summary>
/// A text highlight within a memory.
/// </summary>
public class Highlight
{
    /// <summary>
    /// Unique identifier for the highlight.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Memory ID the highlight belongs to.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Highlighted text.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Start offset in the memory content.
    /// </summary>
    [JsonPropertyName("startOffset")]
    public int StartOffset { get; set; }

    /// <summary>
    /// End offset in the memory content.
    /// </summary>
    [JsonPropertyName("endOffset")]
    public int EndOffset { get; set; }

    /// <summary>
    /// Highlight color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Note associated with the highlight.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// When the highlight was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the highlight was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request to create a highlight.
/// </summary>
public class CreateHighlightRequest
{
    /// <summary>
    /// Highlighted text.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Start offset in the memory content.
    /// </summary>
    [JsonPropertyName("startOffset")]
    public int StartOffset { get; set; }

    /// <summary>
    /// End offset in the memory content.
    /// </summary>
    [JsonPropertyName("endOffset")]
    public int EndOffset { get; set; }

    /// <summary>
    /// Highlight color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Note associated with the highlight.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to update a highlight.
/// </summary>
public class UpdateHighlightRequest
{
    /// <summary>
    /// Highlight color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Note associated with the highlight.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request parameters for extracting highlights.
/// </summary>
public class ExtractHighlightsRequest
{
    /// <summary>
    /// Extraction method.
    /// </summary>
    [JsonPropertyName("method")]
    public ExtractionMethod? Method { get; set; }

    /// <summary>
    /// Maximum number of highlights to extract.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Minimum text length for highlights.
    /// </summary>
    [JsonPropertyName("minLength")]
    public int? MinLength { get; set; }
}

/// <summary>
/// An extracted highlight with score.
/// </summary>
public class ExtractedHighlight
{
    /// <summary>
    /// Highlighted text.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Start offset.
    /// </summary>
    [JsonPropertyName("startOffset")]
    public int StartOffset { get; set; }

    /// <summary>
    /// End offset.
    /// </summary>
    [JsonPropertyName("endOffset")]
    public int EndOffset { get; set; }

    /// <summary>
    /// Relevance score (0-1).
    /// </summary>
    [JsonPropertyName("score")]
    public double Score { get; set; }
}

/// <summary>
/// Result of highlight extraction.
/// </summary>
public class ExtractHighlightsResult
{
    /// <summary>
    /// Memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Extracted highlights.
    /// </summary>
    [JsonPropertyName("highlights")]
    public List<ExtractedHighlight> Highlights { get; set; } = new();
}

/// <summary>
/// Request parameters for listing highlights.
/// </summary>
public class ListHighlightsRequest
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
}

/// <summary>
/// Request for searching highlights.
/// </summary>
public class SearchHighlightsRequest
{
    /// <summary>
    /// Search query.
    /// </summary>
    [JsonPropertyName("query")]
    public required string Query { get; set; }

    /// <summary>
    /// Maximum number of results.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Minimum similarity threshold.
    /// </summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }
}

/// <summary>
/// Highlight search result with similarity score.
/// </summary>
public class HighlightSearchMatch
{
    /// <summary>
    /// The highlight.
    /// </summary>
    [JsonPropertyName("highlight")]
    public Highlight Highlight { get; set; } = null!;

    /// <summary>
    /// Similarity score (0-1).
    /// </summary>
    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }
}

/// <summary>
/// Result of highlight search.
/// </summary>
public class HighlightSearchResult
{
    /// <summary>
    /// Matching highlights with scores.
    /// </summary>
    [JsonPropertyName("matches")]
    public List<HighlightSearchMatch> Matches { get; set; } = new();

    /// <summary>
    /// Total count of matches.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

/// <summary>
/// Highlight type information.
/// </summary>
public class HighlightTypeInfo
{
    /// <summary>
    /// Type name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Default color for this type.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
}

/// <summary>
/// Result of getting highlight types.
/// </summary>
public class HighlightTypesResult
{
    /// <summary>
    /// Available highlight types.
    /// </summary>
    [JsonPropertyName("types")]
    public List<HighlightTypeInfo> Types { get; set; } = new();
}

/// <summary>
/// Request for linking a highlight to a memory.
/// </summary>
public class LinkHighlightRequest
{
    /// <summary>
    /// Memory ID to link to.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public required string MemoryId { get; set; }

    /// <summary>
    /// Optional note about the link.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}

/// <summary>
/// Result of linking a highlight.
/// </summary>
public class HighlightLinkResult
{
    /// <summary>
    /// The highlight ID.
    /// </summary>
    [JsonPropertyName("highlightId")]
    public string HighlightId { get; set; } = string.Empty;

    /// <summary>
    /// The linked memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// When the link was created.
    /// </summary>
    [JsonPropertyName("linkedAt")]
    public DateTime LinkedAt { get; set; }
}
