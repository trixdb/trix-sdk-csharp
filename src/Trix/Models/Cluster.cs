using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents the scale/granularity of a cluster.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ClusterScale
{
    /// <summary>Fine-grained clusters with tightly related memories.</summary>
    [JsonPropertyName("fine")] Fine,

    /// <summary>Medium-grained clusters with moderately related memories.</summary>
    [JsonPropertyName("medium")] Medium,

    /// <summary>Coarse-grained clusters with broadly related memories.</summary>
    [JsonPropertyName("coarse")] Coarse
}

/// <summary>
/// Represents a cluster of related memories.
/// </summary>
public class Cluster
{
    /// <summary>Gets or sets the unique identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>Gets or sets the space this cluster belongs to.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the cluster name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the cluster description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the memory IDs in this cluster.</summary>
    [JsonPropertyName("memoryIds")]
    public List<string>? MemoryIds { get; set; }

    /// <summary>Gets or sets the centroid vector.</summary>
    [JsonPropertyName("centroid")]
    public float[]? Centroid { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Gets or sets the cluster scale/granularity.</summary>
    [JsonPropertyName("scale")]
    public ClusterScale? Scale { get; set; }

    /// <summary>Gets or sets the full memories (when requested with IncludeMemories option).</summary>
    [JsonPropertyName("memories")]
    public List<Memory>? Memories { get; set; }
}

/// <summary>
/// Parameters for creating a cluster.
/// </summary>
public class CreateClusterRequest
{
    /// <summary>Gets or sets the cluster name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>Gets or sets the cluster description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the initial memory IDs.</summary>
    [JsonPropertyName("memoryIds")]
    public List<string>? MemoryIds { get; set; }

    /// <summary>Gets or sets the space ID.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Parameters for updating a cluster.
/// </summary>
public class UpdateClusterRequest
{
    /// <summary>Gets or sets the cluster name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the cluster description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets additional metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Parameters for listing clusters.
/// </summary>
public class ListClustersRequest
{
    /// <summary>Maximum number of results.</summary>
    public int? Limit { get; set; }

    /// <summary>Page number.</summary>
    public int? Page { get; set; }

    /// <summary>Offset for pagination.</summary>
    public int? Offset { get; set; }

    /// <summary>Filter by space ID.</summary>
    public string? SpaceId { get; set; }

    /// <summary>Sort field.</summary>
    public string? SortBy { get; set; }

    /// <summary>Sort order.</summary>
    public string? SortOrder { get; set; }

    /// <summary>Filter by cluster scale (Fine, Medium, or Coarse).</summary>
    public ClusterScale? Scale { get; set; }
}

/// <summary>
/// Options for getting a cluster.
/// </summary>
public class GetClusterOptions
{
    /// <summary>Gets or sets whether to include full memory objects in the response.</summary>
    public bool IncludeMemories { get; set; }
}

/// <summary>
/// Result of cluster expansion.
/// </summary>
public class ExpandResult
{
    /// <summary>Gets or sets the cluster ID.</summary>
    [JsonPropertyName("clusterId")]
    public required string ClusterId { get; set; }

    /// <summary>Gets or sets the suggested new memories.</summary>
    [JsonPropertyName("newMemories")]
    public required List<MemorySuggestion> NewMemories { get; set; }
}

/// <summary>
/// A memory suggestion with confidence score.
/// </summary>
public class MemorySuggestion
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memoryId")]
    public required string MemoryId { get; set; }

    /// <summary>Gets or sets the confidence score.</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }
}

/// <summary>
/// Cluster statistics.
/// </summary>
public class ClusterStats
{
    /// <summary>Gets or sets the total cluster count.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>Gets or sets the average cluster size.</summary>
    [JsonPropertyName("avgSize")]
    public double AvgSize { get; set; }

    /// <summary>Gets or sets the largest cluster size.</summary>
    [JsonPropertyName("maxSize")]
    public int MaxSize { get; set; }

    /// <summary>Gets or sets the smallest cluster size.</summary>
    [JsonPropertyName("minSize")]
    public int MinSize { get; set; }

    /// <summary>Gets or sets the total memories in clusters.</summary>
    [JsonPropertyName("totalMemories")]
    public int TotalMemories { get; set; }
}

/// <summary>
/// Cluster quality metrics.
/// </summary>
public class ClusterQuality
{
    /// <summary>Gets or sets the cluster ID.</summary>
    [JsonPropertyName("clusterId")]
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>Gets or sets the cohesion score.</summary>
    [JsonPropertyName("cohesion")]
    public double Cohesion { get; set; }

    /// <summary>Gets or sets the separation score.</summary>
    [JsonPropertyName("separation")]
    public double Separation { get; set; }

    /// <summary>Gets or sets the silhouette score.</summary>
    [JsonPropertyName("silhouette")]
    public double Silhouette { get; set; }
}

/// <summary>
/// Cluster topics.
/// </summary>
public class ClusterTopics
{
    /// <summary>Gets or sets the cluster ID.</summary>
    [JsonPropertyName("clusterId")]
    public string ClusterId { get; set; } = string.Empty;

    /// <summary>Gets or sets the topics.</summary>
    [JsonPropertyName("topics")]
    public List<TopicInfo> Topics { get; set; } = new();
}

/// <summary>
/// Topic information.
/// </summary>
public class TopicInfo
{
    /// <summary>Gets or sets the topic name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the topic weight.</summary>
    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    /// <summary>Gets or sets related keywords.</summary>
    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }
}

/// <summary>
/// Parameters for incremental clustering.
/// </summary>
public class IncrementalClusterRequest
{
    /// <summary>Gets or sets the space ID filter.</summary>
    [JsonPropertyName("spaceId")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the similarity threshold.</summary>
    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }

    /// <summary>Gets or sets the maximum clusters to create.</summary>
    [JsonPropertyName("maxClusters")]
    public int? MaxClusters { get; set; }
}

/// <summary>
/// Result of incremental clustering.
/// </summary>
public class IncrementalClusterResult
{
    /// <summary>Gets or sets the number of new clusters created.</summary>
    [JsonPropertyName("clustersCreated")]
    public int ClustersCreated { get; set; }

    /// <summary>Gets or sets the number of memories assigned.</summary>
    [JsonPropertyName("memoriesAssigned")]
    public int MemoriesAssigned { get; set; }

    /// <summary>Gets or sets the cluster IDs affected.</summary>
    [JsonPropertyName("clusterIds")]
    public List<string> ClusterIds { get; set; } = new();
}
