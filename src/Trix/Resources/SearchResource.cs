using System.Text.Json;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for semantic search and embedding operations.
/// </summary>
public class SearchResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the SearchResource.
    /// </summary>
    internal SearchResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Searches for memories using semantic search.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="request">Optional search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of memories matching the search query.</returns>
    public virtual async Task<List<Memory>> SearchAsync(
        string query,
        SearchRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(query);

        var queryParams = new Dictionary<string, string?>
        {
            ["q"] = query
        };

        if (request != null)
        {
            if (request.Limit != null) queryParams["limit"] = request.Limit.ToString();
            if (request.SpaceId != null) queryParams["space_id"] = request.SpaceId;
            if (request.Threshold != null) queryParams["threshold"] = request.Threshold.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (request.Tags != null && request.Tags.Count > 0)
            {
                queryParams["tags"] = string.Join(",", request.Tags);
            }
        }

        var response = await GetAsync<PaginatedResponse<Memory>>("/v1/search", queryParams, cancellationToken)
            .ConfigureAwait(false);
        return response.Data;
    }

    /// <summary>
    /// Finds memories similar to a given memory.
    /// </summary>
    /// <param name="memoryId">Memory ID to find similar memories for.</param>
    /// <param name="request">Search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Similar memories with similarity scores.</returns>
    public virtual async Task<SimilarityResult> SimilarAsync(
        string memoryId,
        SimilarRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var queryParams = BuildQueryParams(
            ("limit", request?.Limit),
            ("threshold", request?.Threshold),
            ("includeEmbedding", request?.IncludeEmbedding),
            ("spaceId", request?.SpaceId),
            ("clusterScale", request?.ClusterScale?.ToString()?.ToLowerInvariant())
        );

        return await GetAsync<SimilarityResult>($"/v1/search/similar/{Uri.EscapeDataString(memoryId)}", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Generates embeddings for specific memories.
    /// </summary>
    /// <param name="memoryIds">Memory IDs to generate embeddings for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The embeddings for the memories.</returns>
    public virtual async Task<EmbedResult> EmbedAsync(
        IEnumerable<string> memoryIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(memoryIds);

        var request = new { memoryIds = memoryIds.ToList() };
        return await PostAsync<EmbedResult>("/v1/search/embed", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers embedding generation for all memories that need it.
    /// </summary>
    /// <param name="batchSize">Batch size for processing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The batch embedding result.</returns>
    public virtual async Task<EmbedAllResult> EmbedAllAsync(
        int? batchSize = null,
        CancellationToken cancellationToken = default)
    {
        var request = batchSize.HasValue ? new { batchSize = batchSize.Value } : null;
        return await PostAsync<EmbedAllResult>("/v1/search/embed-all", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the search system configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The search configuration.</returns>
    public virtual async Task<SearchConfig> GetConfigAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<SearchConfig>("/v1/search/config", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Executes multiple searches in a single request.
    /// </summary>
    /// <param name="searches">Array of search objects.</param>
    /// <param name="deduplicate">Whether to deduplicate results across searches.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The batch search results.</returns>
    public virtual async Task<JsonElement> BatchSearchAsync(
        object[] searches,
        bool deduplicate = true,
        CancellationToken cancellationToken = default)
    {
        var request = new { searches, deduplicate };
        return await PostAsync<JsonElement>("/v1/search/batch", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Suggests an optimal search strategy for a query.
    /// </summary>
    /// <param name="query">The query to suggest a strategy for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The suggested search strategy.</returns>
    public virtual async Task<JsonElement> SuggestStrategyAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(query);
        var request = new { query };
        return await PostAsync<JsonElement>("/v1/search/suggest-strategy", request, cancellationToken)
            .ConfigureAwait(false);
    }
}
