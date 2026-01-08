using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// A search result with similarity score.
/// </summary>
public class SimilarMemory
{
    /// <summary>
    /// The matching memory.
    /// </summary>
    [JsonPropertyName("memory")]
    public required Memory Memory { get; set; }

    /// <summary>
    /// Similarity score (0-1).
    /// </summary>
    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }
}

/// <summary>
/// Result of a similarity search.
/// </summary>
public class SimilarityResult
{
    /// <summary>
    /// The source memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Similar memories ordered by similarity.
    /// </summary>
    [JsonPropertyName("results")]
    public List<SimilarMemory> Results { get; set; } = new();
}

/// <summary>
/// Request parameters for similarity search.
/// </summary>
public class SimilarRequest
{
    /// <summary>
    /// Maximum number of results to return.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Minimum similarity threshold (0-1).
    /// </summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>
    /// Include embeddings in response.
    /// </summary>
    [JsonPropertyName("includeEmbedding")]
    public bool? IncludeEmbedding { get; set; }

    /// <summary>
    /// Filter by space ID.
    /// </summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Cluster scale for search (Fine, Medium, or Coarse).
    /// </summary>
    [JsonPropertyName("clusterScale")]
    public ClusterScale? ClusterScale { get; set; }
}

/// <summary>
/// Request parameters for semantic search.
/// </summary>
public class SearchRequest
{
    /// <summary>
    /// The search query.
    /// </summary>
    [JsonPropertyName("query")]
    public required string Query { get; set; }

    /// <summary>
    /// Maximum number of results to return.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Cluster scale for search (Fine, Medium, or Coarse).
    /// </summary>
    [JsonPropertyName("clusterScale")]
    public ClusterScale? ClusterScale { get; set; }

    /// <summary>
    /// Filter by space ID.
    /// </summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Filter by tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Minimum similarity threshold (0-1).
    /// </summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }
}

/// <summary>
/// An embedding for a memory.
/// </summary>
public class MemoryEmbedding
{
    /// <summary>
    /// The memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// The embedding vector.
    /// </summary>
    [JsonPropertyName("embedding")]
    public double[] Embedding { get; set; } = Array.Empty<double>();
}

/// <summary>
/// Result of embedding generation.
/// </summary>
public class EmbedResult
{
    /// <summary>
    /// Embeddings for the requested memories.
    /// </summary>
    [JsonPropertyName("embeddings")]
    public List<MemoryEmbedding> Embeddings { get; set; } = new();
}

/// <summary>
/// Result of batch embedding operation.
/// </summary>
public class EmbedAllResult
{
    /// <summary>
    /// Total memories to embed.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Number of memories processed.
    /// </summary>
    [JsonPropertyName("processed")]
    public int Processed { get; set; }

    /// <summary>
    /// Job ID for tracking the operation.
    /// </summary>
    [JsonPropertyName("jobId")]
    public string? JobId { get; set; }
}

/// <summary>
/// Search system configuration.
/// </summary>
public class SearchConfig
{
    /// <summary>
    /// Embedding model being used.
    /// </summary>
    [JsonPropertyName("embeddingModel")]
    public string EmbeddingModel { get; set; } = string.Empty;

    /// <summary>
    /// Dimensionality of embeddings.
    /// </summary>
    [JsonPropertyName("embeddingDimensions")]
    public int EmbeddingDimensions { get; set; }

    /// <summary>
    /// Maximum batch size for embedding requests.
    /// </summary>
    [JsonPropertyName("maxBatchSize")]
    public int MaxBatchSize { get; set; }
}
