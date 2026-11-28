using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents a paginated response from the API.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>Gets or sets the data items.</summary>
    [JsonPropertyName("data")]
    public required List<T> Data { get; set; }

    /// <summary>Gets or sets the pagination metadata.</summary>
    [JsonPropertyName("pagination")]
    public PaginationMetadata? Pagination { get; set; }
}

/// <summary>
/// Pagination metadata.
/// </summary>
public class PaginationMetadata
{
    /// <summary>Gets or sets the total number of items. Omitted by the API when include_total_count is false.</summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>Gets or sets the current page number.</summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>Gets or sets the number of items per page.</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>Gets or sets whether there are more pages.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    /// <summary>Gets or sets the cursor for the next page in keyset pagination.</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
