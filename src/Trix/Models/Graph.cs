using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Direction for graph traversal.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<TraversalDirection>))]
public enum TraversalDirection
{
    [JsonPropertyName("outgoing")]
    Outgoing,
    [JsonPropertyName("incoming")]
    Incoming,
    [JsonPropertyName("both")]
    Both
}

/// <summary>
/// A node in the knowledge graph.
/// </summary>
public class GraphNode
{
    /// <summary>
    /// Unique identifier for the node.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Type of the node (memory, entity, etc.).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Node data (memory content, entity properties, etc.).
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// Depth from the starting node in traversal.
    /// </summary>
    [JsonPropertyName("depth")]
    public int Depth { get; set; }
}

/// <summary>
/// An edge in the knowledge graph.
/// </summary>
public class GraphEdge
{
    /// <summary>
    /// Source node ID.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Target node ID.
    /// </summary>
    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// Relationship type.
    /// </summary>
    [JsonPropertyName("relationshipType")]
    public string RelationshipType { get; set; } = string.Empty;

    /// <summary>
    /// Relationship strength (0-1).
    /// </summary>
    [JsonPropertyName("strength")]
    public double Strength { get; set; }
}

/// <summary>
/// Result of a graph traversal operation.
/// </summary>
public class GraphTraversalResult
{
    /// <summary>
    /// Nodes discovered during traversal.
    /// </summary>
    [JsonPropertyName("nodes")]
    public List<GraphNode> Nodes { get; set; } = new();

    /// <summary>
    /// Edges discovered during traversal.
    /// </summary>
    [JsonPropertyName("edges")]
    public List<GraphEdge> Edges { get; set; } = new();
}

/// <summary>
/// Request parameters for graph traversal.
/// </summary>
public class TraverseRequest
{
    /// <summary>
    /// Starting node ID for traversal.
    /// </summary>
    [JsonPropertyName("startNodeId")]
    public string StartNodeId { get; set; } = string.Empty;

    /// <summary>
    /// Maximum depth to traverse (default: 2).
    /// </summary>
    [JsonPropertyName("maxDepth")]
    public int? MaxDepth { get; set; }

    /// <summary>
    /// Relationship types to follow.
    /// </summary>
    [JsonPropertyName("relationshipTypes")]
    public List<string>? RelationshipTypes { get; set; }

    /// <summary>
    /// Direction of traversal.
    /// </summary>
    [JsonPropertyName("direction")]
    public TraversalDirection? Direction { get; set; }

    /// <summary>
    /// Maximum number of nodes to return.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

/// <summary>
/// Context result containing a memory and its related information.
/// </summary>
public class ContextResult
{
    /// <summary>
    /// The central memory.
    /// </summary>
    [JsonPropertyName("central")]
    public required Memory Central { get; set; }

    /// <summary>
    /// Related memories.
    /// </summary>
    [JsonPropertyName("related")]
    public List<Memory> Related { get; set; } = [];

    /// <summary>
    /// Relationships between memories.
    /// </summary>
    [JsonPropertyName("relationships")]
    public List<Relationship> Relationships { get; set; } = new();

    /// <summary>
    /// Clusters containing the memory.
    /// </summary>
    [JsonPropertyName("clusters")]
    public List<Cluster>? Clusters { get; set; }
}

/// <summary>
/// Request parameters for getting context.
/// </summary>
public class GetContextRequest
{
    /// <summary>
    /// Memory ID to get context for.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Depth of context to retrieve (default: 2).
    /// </summary>
    [JsonPropertyName("depth")]
    public int? Depth { get; set; }

    /// <summary>
    /// Relationship types to include.
    /// </summary>
    [JsonPropertyName("relationshipTypes")]
    public List<string>? RelationshipTypes { get; set; }

    /// <summary>
    /// Include metadata in response.
    /// </summary>
    [JsonPropertyName("includeMetadata")]
    public bool? IncludeMetadata { get; set; }
}

/// <summary>
/// A step in a path between nodes.
/// </summary>
public class PathStep
{
    /// <summary>
    /// The node at this step.
    /// </summary>
    [JsonPropertyName("node")]
    public GraphNode Node { get; set; } = new();

    /// <summary>
    /// The relationship taken to reach this node (null for first node).
    /// </summary>
    [JsonPropertyName("relationship")]
    public GraphEdge? Relationship { get; set; }
}

/// <summary>
/// Result of a shortest path query.
/// </summary>
public class ShortestPathResult
{
    /// <summary>
    /// Whether a path was found.
    /// </summary>
    [JsonPropertyName("found")]
    public bool Found { get; set; }

    /// <summary>
    /// Distance (number of hops) if found.
    /// </summary>
    [JsonPropertyName("distance")]
    public int? Distance { get; set; }

    /// <summary>
    /// The path from source to target.
    /// </summary>
    [JsonPropertyName("path")]
    public List<PathStep>? Path { get; set; }
}

/// <summary>
/// A neighbor node with its connecting relationship.
/// </summary>
public class GraphNeighbor
{
    /// <summary>
    /// The neighbor node.
    /// </summary>
    [JsonPropertyName("node")]
    public GraphNode Node { get; set; } = new();

    /// <summary>
    /// The relationship connecting to this neighbor.
    /// </summary>
    [JsonPropertyName("relationship")]
    public GraphEdge Relationship { get; set; } = new();
}

/// <summary>
/// Result of a neighbors query.
/// </summary>
public class GraphNeighborsResult
{
    /// <summary>
    /// The node for which neighbors were retrieved.
    /// </summary>
    [JsonPropertyName("nodeId")]
    public string NodeId { get; set; } = string.Empty;

    /// <summary>
    /// The neighboring nodes.
    /// </summary>
    [JsonPropertyName("neighbors")]
    public List<GraphNeighbor> Neighbors { get; set; } = new();
}

/// <summary>
/// Statistics about the knowledge graph.
/// </summary>
public class GraphStats
{
    /// <summary>
    /// Total number of nodes.
    /// </summary>
    [JsonPropertyName("nodeCount")]
    public int NodeCount { get; set; }

    /// <summary>
    /// Total number of edges.
    /// </summary>
    [JsonPropertyName("edgeCount")]
    public int EdgeCount { get; set; }

    /// <summary>
    /// Average degree (connections per node).
    /// </summary>
    [JsonPropertyName("avgDegree")]
    public double AvgDegree { get; set; }

    /// <summary>
    /// Graph density (0-1).
    /// </summary>
    [JsonPropertyName("density")]
    public double Density { get; set; }

    /// <summary>
    /// Number of connected components.
    /// </summary>
    [JsonPropertyName("components")]
    public int Components { get; set; }
}

/// <summary>
/// Weights used in hybrid scoring.
/// </summary>
public class HybridScoringWeights
{
    /// <summary>
    /// Semantic similarity weight.
    /// </summary>
    [JsonPropertyName("semantic")]
    public double Semantic { get; set; }

    /// <summary>
    /// Graph proximity weight.
    /// </summary>
    [JsonPropertyName("graph")]
    public double Graph { get; set; }

    /// <summary>
    /// Co-activation strength weight.
    /// </summary>
    [JsonPropertyName("coActivation")]
    public double CoActivation { get; set; }

    /// <summary>
    /// Recency weight.
    /// </summary>
    [JsonPropertyName("recency")]
    public double Recency { get; set; }

    /// <summary>
    /// Salience weight.
    /// </summary>
    [JsonPropertyName("salience")]
    public double Salience { get; set; }
}

/// <summary>
/// Scoring metadata for graph expansion.
/// </summary>
public class GraphExpansionScoring
{
    /// <summary>
    /// Whether hybrid scoring was applied.
    /// </summary>
    [JsonPropertyName("applied")]
    public bool Applied { get; set; }

    /// <summary>
    /// Scoring weights if applied.
    /// </summary>
    [JsonPropertyName("weights")]
    public HybridScoringWeights? Weights { get; set; }
}

/// <summary>
/// Statistics from graph expansion.
/// </summary>
public class GraphExpansionStats
{
    /// <summary>
    /// Number of seed memories.
    /// </summary>
    [JsonPropertyName("seedCount")]
    public int SeedCount { get; set; }

    /// <summary>
    /// Number of memories discovered.
    /// </summary>
    [JsonPropertyName("expandedCount")]
    public int ExpandedCount { get; set; }

    /// <summary>
    /// Total number of memories after filtering.
    /// </summary>
    [JsonPropertyName("finalCount")]
    public int FinalCount { get; set; }

    /// <summary>
    /// Number of relationships found.
    /// </summary>
    [JsonPropertyName("relationshipsFound")]
    public int? RelationshipsFound { get; set; }

    /// <summary>
    /// Number of hops used.
    /// </summary>
    [JsonPropertyName("hopsUsed")]
    public int? HopsUsed { get; set; }
}

/// <summary>
/// Result of graph expansion from seed memories.
/// </summary>
public class GraphExpansionResult
{
    /// <summary>
    /// Seed memory IDs.
    /// </summary>
    [JsonPropertyName("seedMemories")]
    public List<string> SeedMemories { get; set; } = new();

    /// <summary>
    /// Expanded memories discovered.
    /// </summary>
    [JsonPropertyName("expandedMemories")]
    public List<Memory> ExpandedMemories { get; set; } = new();

    /// <summary>
    /// Relationships in the expansion.
    /// </summary>
    [JsonPropertyName("relationships")]
    public List<Relationship> Relationships { get; set; } = new();

    /// <summary>
    /// Expansion statistics.
    /// </summary>
    [JsonPropertyName("stats")]
    public required GraphExpansionStats Stats { get; set; }

    /// <summary>
    /// Scoring metadata if hybrid scoring was applied.
    /// </summary>
    [JsonPropertyName("scoring")]
    public GraphExpansionScoring? Scoring { get; set; }
}

/// <summary>
/// Request parameters for graph expansion.
/// </summary>
public class ExpandGraphRequest
{
    /// <summary>
    /// Starting memory IDs for expansion.
    /// </summary>
    [JsonPropertyName("seedMemoryIds")]
    public required List<string> SeedMemoryIds { get; set; }

    /// <summary>
    /// Maximum traversal depth (default: 2).
    /// </summary>
    [JsonPropertyName("maxHops")]
    public int? MaxHops { get; set; }

    /// <summary>
    /// Minimum relationship strength (default: 0.3).
    /// </summary>
    [JsonPropertyName("minWeight")]
    public double? MinWeight { get; set; }

    /// <summary>
    /// Filter by specific relationship types.
    /// </summary>
    [JsonPropertyName("relationshipTypes")]
    public List<string>? RelationshipTypes { get; set; }

    /// <summary>
    /// Traversal direction (default: both).
    /// </summary>
    [JsonPropertyName("direction")]
    public string? Direction { get; set; }

    /// <summary>
    /// Include full memory content (default: true).
    /// </summary>
    [JsonPropertyName("includeContent")]
    public bool? IncludeContent { get; set; }

    /// <summary>
    /// Enable multi-signal hybrid scoring (default: false).
    /// </summary>
    [JsonPropertyName("applyHybridScoring")]
    public bool? ApplyHybridScoring { get; set; }
}
