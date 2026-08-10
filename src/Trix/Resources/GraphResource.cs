using System.Runtime.CompilerServices;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for knowledge graph traversal and analysis.
/// </summary>
public class GraphResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the GraphResource.
    /// </summary>
    internal GraphResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Traverses the knowledge graph from starting nodes.
    /// </summary>
    /// <param name="request">Traversal parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The traversal result with nodes and edges.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when start node IDs list is empty.</exception>
    public virtual async Task<GraphTraversalResult> TraverseAsync(
        TraverseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.StartNodeIds == null || request.StartNodeIds.Count == 0)
        {
            throw new ArgumentException("Start node IDs cannot be empty.", nameof(request));
        }

        return await PostAsync<GraphTraversalResult>("/v1/graph/traverse", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets contextual information using semantic search as starting point.
    /// </summary>
    /// <param name="request">Context request parameters including semantic query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The context result with semantic matches and their graph neighbors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when query is empty.</exception>
    public virtual async Task<ContextResult> GetContextAsync(
        GetContextRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrEmpty(request.Query))
        {
            throw new ArgumentException("Query cannot be empty.", nameof(request));
        }

        return await PostAsync<ContextResult>("/v1/graph/context", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Finds the shortest path between two nodes.
    /// </summary>
    /// <param name="sourceId">Source node ID.</param>
    /// <param name="targetId">Target node ID.</param>
    /// <param name="relationshipTypes">Relationship types to follow.</param>
    /// <param name="maxDepth">Maximum depth to search.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The shortest path result.</returns>
    public virtual async Task<ShortestPathResult> ShortestPathAsync(
        string sourceId,
        string targetId,
        List<string>? relationshipTypes = null,
        int? maxDepth = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sourceId);
        ArgumentException.ThrowIfNullOrEmpty(targetId);

        var request = new
        {
            source_id = sourceId,
            target_id = targetId,
            relationship_types = relationshipTypes,
            max_depth = maxDepth
        };

        return await PostAsync<ShortestPathResult>("/v1/graph/path", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the immediate neighbors of a node.
    /// </summary>
    /// <param name="nodeId">Node ID.</param>
    /// <param name="direction">Direction of relationships.</param>
    /// <param name="limit">Maximum number of neighbors to return.</param>
    /// <param name="depth">Depth of neighbor traversal (1-3).</param>
    /// <param name="types">Relationship types to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The neighbors result.</returns>
    public virtual async Task<GraphNeighborsResult> NeighborsAsync(
        string nodeId,
        TraversalDirection? direction = null,
        int? limit = null,
        int? depth = null,
        List<string>? types = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(nodeId);

        var queryParams = BuildQueryParams(
            ("direction", direction?.ToWireValue()),
            ("limit", limit),
            ("depth", depth),
            ("types", types != null ? string.Join(",", types) : null)
        );

        return await GetAsync<GraphNeighborsResult>($"/v1/graph/neighbors/{Uri.EscapeDataString(nodeId)}", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets statistics about the knowledge graph.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Graph statistics.</returns>
    public virtual async Task<GraphStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<GraphStats>("/v1/graph/stats", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Expands graph from seed memories using traversal and optional hybrid scoring.
    /// </summary>
    /// <param name="request">Expansion parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Graph expansion result with expanded memories, relationships, and stats.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when seed memory IDs list is empty.</exception>
    /// <example>
    /// <code>
    /// // Basic expansion
    /// var result = await client.Graph.ExpandAsync(new ExpandGraphRequest
    /// {
    ///     SeedMemoryIds = new List&lt;string&gt; { "mem_123", "mem_456" }
    /// });
    /// Console.WriteLine($"Expanded to {result.Stats.ExpandedCount} memories");
    ///
    /// // Advanced expansion with hybrid scoring
    /// var result = await client.Graph.ExpandAsync(new ExpandGraphRequest
    /// {
    ///     SeedMemoryIds = new List&lt;string&gt; { "mem_123" },
    ///     MaxHops = 3,
    ///     MinWeight = 0.5,
    ///     RelationshipTypes = new List&lt;string&gt; { "related_to", "supports" },
    ///     ApplyHybridScoring = true
    /// });
    /// Console.WriteLine($"Scoring applied: {result.Scoring?.Applied}");
    /// </code>
    /// </example>
    public virtual async Task<GraphExpansionResult> ExpandAsync(
        ExpandGraphRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.SeedMemoryIds == null || request.SeedMemoryIds.Count == 0)
        {
            throw new ArgumentException("Seed memory IDs cannot be empty.", nameof(request));
        }

        return await PostAsync<GraphExpansionResult>("/v1/graph/expand", request, cancellationToken)
            .ConfigureAwait(false);
    }
}
