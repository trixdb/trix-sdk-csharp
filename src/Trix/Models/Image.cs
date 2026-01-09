using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents the type of similarity search.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SimilarityType
{
    /// <summary>Text/content-based similarity.</summary>
    [JsonPropertyName("text")] Text,

    /// <summary>Image/visual similarity.</summary>
    [JsonPropertyName("image")] Image
}

/// <summary>
/// Result of a visual search operation.
/// </summary>
public class VisualSearchResult
{
    /// <summary>Gets or sets the matching memories.</summary>
    [JsonPropertyName("results")]
    public List<VisualSearchMatch> Results { get; set; } = new();

    /// <summary>Gets or sets the total count of results.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

/// <summary>
/// A match from visual search.
/// </summary>
public class VisualSearchMatch
{
    /// <summary>Gets or sets the matching memory.</summary>
    [JsonPropertyName("memory")]
    public required Memory Memory { get; set; }

    /// <summary>Gets or sets the visual similarity score (0-1).</summary>
    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }

    /// <summary>Gets or sets the distance metric value.</summary>
    [JsonPropertyName("distance")]
    public double? Distance { get; set; }
}

/// <summary>
/// Request parameters for visual search.
/// </summary>
public class VisualSearchRequest
{
    /// <summary>Gets or sets the maximum number of results.</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>Gets or sets the minimum similarity threshold (0-1).</summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>Gets or sets the space ID filter.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets whether to include embeddings in results.</summary>
    [JsonPropertyName("includeEmbedding")]
    public bool? IncludeEmbedding { get; set; }
}

/// <summary>
/// Request parameters for text-to-image search.
/// </summary>
public class TextToImageSearchRequest
{
    /// <summary>Gets or sets the text query.</summary>
    [JsonPropertyName("query")]
    public required string Query { get; set; }

    /// <summary>Gets or sets the maximum number of results.</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>Gets or sets the minimum similarity threshold (0-1).</summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>Gets or sets the space ID filter.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets filter tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Request parameters for finding similar images.
/// </summary>
public class FindSimilarImagesRequest
{
    /// <summary>Gets or sets the similarity type (text or image).</summary>
    [JsonPropertyName("type")]
    public SimilarityType? Type { get; set; }

    /// <summary>Gets or sets the maximum number of results.</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>Gets or sets the minimum similarity threshold (0-1).</summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>Gets or sets the space ID filter.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets whether to include embeddings in results.</summary>
    [JsonPropertyName("includeEmbedding")]
    public bool? IncludeEmbedding { get; set; }
}

/// <summary>
/// Result of duplicate check operation.
/// </summary>
public class DuplicateCheckResult
{
    /// <summary>Gets or sets whether duplicates were found.</summary>
    [JsonPropertyName("hasDuplicates")]
    public bool HasDuplicates { get; set; }

    /// <summary>Gets or sets the duplicate memories found.</summary>
    [JsonPropertyName("duplicates")]
    public List<DuplicateMatch> Duplicates { get; set; } = new();

    /// <summary>Gets or sets the exact match if found.</summary>
    [JsonPropertyName("exactMatch")]
    public Memory? ExactMatch { get; set; }
}

/// <summary>
/// A duplicate match with similarity score.
/// </summary>
public class DuplicateMatch
{
    /// <summary>Gets or sets the matching memory.</summary>
    [JsonPropertyName("memory")]
    public required Memory Memory { get; set; }

    /// <summary>Gets or sets the similarity score (0-1).</summary>
    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }

    /// <summary>Gets or sets whether this is an exact match.</summary>
    [JsonPropertyName("isExact")]
    public bool IsExact { get; set; }
}

/// <summary>
/// Request parameters for checking duplicates.
/// </summary>
public class CheckDuplicatesRequest
{
    /// <summary>Gets or sets the similarity threshold for duplicates (0-1).</summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>Gets or sets the space ID filter.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the maximum number of duplicates to return.</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

/// <summary>
/// Request parameters for image clustering.
/// </summary>
public class ClusterImagesRequest
{
    /// <summary>Gets or sets the space ID filter.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the number of clusters to create.</summary>
    [JsonPropertyName("numClusters")]
    public int? NumClusters { get; set; }

    /// <summary>Gets or sets the minimum cluster size.</summary>
    [JsonPropertyName("minClusterSize")]
    public int? MinClusterSize { get; set; }

    /// <summary>Gets or sets the clustering algorithm.</summary>
    [JsonPropertyName("algorithm")]
    public string? Algorithm { get; set; }
}

/// <summary>
/// Result of image clustering operation.
/// </summary>
public class ImageClusterResult
{
    /// <summary>Gets or sets the clusters created.</summary>
    [JsonPropertyName("clusters")]
    public List<ImageCluster> Clusters { get; set; } = new();

    /// <summary>Gets or sets the total images clustered.</summary>
    [JsonPropertyName("totalImages")]
    public int TotalImages { get; set; }

    /// <summary>Gets or sets the number of unclustered images.</summary>
    [JsonPropertyName("unclustered")]
    public int Unclustered { get; set; }

    /// <summary>Gets or sets the job ID if processing asynchronously.</summary>
    [JsonPropertyName("jobId")]
    public string? JobId { get; set; }
}

/// <summary>
/// A cluster of visually similar images.
/// </summary>
public class ImageCluster
{
    /// <summary>Gets or sets the cluster ID.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the cluster name/label.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the memory IDs in this cluster.</summary>
    [JsonPropertyName("memoryIds")]
    public List<string> MemoryIds { get; set; } = new();

    /// <summary>Gets or sets the representative image ID.</summary>
    [JsonPropertyName("representativeId")]
    public string? RepresentativeId { get; set; }

    /// <summary>Gets or sets the average similarity within the cluster.</summary>
    [JsonPropertyName("avgSimilarity")]
    public double? AvgSimilarity { get; set; }

    /// <summary>Gets or sets suggested tags for this cluster.</summary>
    [JsonPropertyName("suggestedTags")]
    public List<string>? SuggestedTags { get; set; }
}

/// <summary>
/// Result of auto-tagging an image.
/// </summary>
public class AutoTagResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>Gets or sets the generated tags.</summary>
    [JsonPropertyName("tags")]
    public List<GeneratedTag> Tags { get; set; } = new();

    /// <summary>Gets or sets the detected objects.</summary>
    [JsonPropertyName("objects")]
    public List<DetectedObject>? Objects { get; set; }

    /// <summary>Gets or sets the detected scene/context.</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>Gets or sets detected colors.</summary>
    [JsonPropertyName("colors")]
    public List<DetectedColor>? Colors { get; set; }

    /// <summary>Gets or sets whether the image contains text.</summary>
    [JsonPropertyName("hasText")]
    public bool? HasText { get; set; }

    /// <summary>Gets or sets extracted text from the image (OCR).</summary>
    [JsonPropertyName("extractedText")]
    public string? ExtractedText { get; set; }
}

/// <summary>
/// A generated tag with confidence score.
/// </summary>
public class GeneratedTag
{
    /// <summary>Gets or sets the tag name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the confidence score (0-1).</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    /// <summary>Gets or sets the tag category.</summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }
}

/// <summary>
/// A detected object in an image.
/// </summary>
public class DetectedObject
{
    /// <summary>Gets or sets the object label.</summary>
    [JsonPropertyName("label")]
    public required string Label { get; set; }

    /// <summary>Gets or sets the confidence score (0-1).</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    /// <summary>Gets or sets the bounding box (x, y, width, height as percentages).</summary>
    [JsonPropertyName("boundingBox")]
    public BoundingBox? BoundingBox { get; set; }
}

/// <summary>
/// Bounding box coordinates (as percentages of image dimensions).
/// </summary>
public class BoundingBox
{
    /// <summary>Gets or sets the X coordinate (0-1).</summary>
    [JsonPropertyName("x")]
    public double X { get; set; }

    /// <summary>Gets or sets the Y coordinate (0-1).</summary>
    [JsonPropertyName("y")]
    public double Y { get; set; }

    /// <summary>Gets or sets the width (0-1).</summary>
    [JsonPropertyName("width")]
    public double Width { get; set; }

    /// <summary>Gets or sets the height (0-1).</summary>
    [JsonPropertyName("height")]
    public double Height { get; set; }
}

/// <summary>
/// A detected color in an image.
/// </summary>
public class DetectedColor
{
    /// <summary>Gets or sets the color name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color code.</summary>
    [JsonPropertyName("hex")]
    public required string Hex { get; set; }

    /// <summary>Gets or sets the percentage of image area.</summary>
    [JsonPropertyName("percentage")]
    public double Percentage { get; set; }
}

/// <summary>
/// Request parameters for batch auto-tagging.
/// </summary>
public class BatchAutoTagRequest
{
    /// <summary>Gets or sets the image/memory IDs to auto-tag.</summary>
    [JsonPropertyName("imageIds")]
    public required List<string> ImageIds { get; set; }

    /// <summary>Gets or sets whether to apply generated tags to memories.</summary>
    [JsonPropertyName("applyTags")]
    public bool? ApplyTags { get; set; }

    /// <summary>Gets or sets the minimum confidence for applied tags.</summary>
    [JsonPropertyName("minConfidence")]
    public double? MinConfidence { get; set; }
}

/// <summary>
/// Result of batch auto-tagging.
/// </summary>
public class BatchAutoTagResult
{
    /// <summary>Gets or sets the results for each image.</summary>
    [JsonPropertyName("results")]
    public List<AutoTagResult> Results { get; set; } = new();

    /// <summary>Gets or sets the number of successful operations.</summary>
    [JsonPropertyName("success")]
    public int Success { get; set; }

    /// <summary>Gets or sets the number of failed operations.</summary>
    [JsonPropertyName("failed")]
    public int Failed { get; set; }

    /// <summary>Gets or sets any errors that occurred.</summary>
    [JsonPropertyName("errors")]
    public List<BatchAutoTagError>? Errors { get; set; }
}

/// <summary>
/// Error from batch auto-tag operation.
/// </summary>
public class BatchAutoTagError
{
    /// <summary>Gets or sets the image/memory ID that failed.</summary>
    [JsonPropertyName("imageId")]
    public required string ImageId { get; set; }

    /// <summary>Gets or sets the error message.</summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}

/// <summary>
/// Request parameters for query suggestions.
/// </summary>
public class SuggestQueriesRequest
{
    /// <summary>Gets or sets the space ID filter.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the maximum number of suggestions.</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>Gets or sets the suggestion categories to include.</summary>
    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }
}

/// <summary>
/// Result of query suggestions.
/// </summary>
public class QuerySuggestionsResult
{
    /// <summary>Gets or sets the suggested queries.</summary>
    [JsonPropertyName("suggestions")]
    public List<QuerySuggestion> Suggestions { get; set; } = new();
}

/// <summary>
/// A suggested query.
/// </summary>
public class QuerySuggestion
{
    /// <summary>Gets or sets the suggested query text.</summary>
    [JsonPropertyName("query")]
    public required string Query { get; set; }

    /// <summary>Gets or sets the category of the suggestion.</summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>Gets or sets the expected result count.</summary>
    [JsonPropertyName("estimatedResults")]
    public int? EstimatedResults { get; set; }

    /// <summary>Gets or sets example matching memory IDs.</summary>
    [JsonPropertyName("exampleIds")]
    public List<string>? ExampleIds { get; set; }
}

/// <summary>
/// Request parameters for creating an image memory.
/// </summary>
public class CreateImageMemoryRequest
{
    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets whether to auto-tag the image.</summary>
    [JsonPropertyName("autoTag")]
    public bool? AutoTag { get; set; }

    /// <summary>Gets or sets whether to extract text (OCR).</summary>
    [JsonPropertyName("extractText")]
    public bool? ExtractText { get; set; }

    /// <summary>Gets or sets a description/caption for the image.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
