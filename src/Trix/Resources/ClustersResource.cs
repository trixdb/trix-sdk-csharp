using System.Runtime.CompilerServices;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Provides methods for managing memory clusters.
/// </summary>
public class ClustersResource : BaseResource
{
    private const string BasePath = "/v1/clusters";

    internal ClustersResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Creates a new cluster.
    /// </summary>
    public virtual async Task<Cluster> CreateAsync(
        CreateClusterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Cluster>(BasePath, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new cluster with the specified name.
    /// </summary>
    public virtual async Task<Cluster> CreateAsync(
        string name,
        string? description = null,
        List<string>? memoryIds = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateAsync(new CreateClusterRequest
        {
            Name = name,
            Description = description,
            MemoryIds = memoryIds
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a cluster by ID.
    /// </summary>
    /// <param name="id">The cluster ID.</param>
    /// <param name="options">Options for retrieving the cluster.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cluster.</returns>
    public virtual async Task<Cluster> GetAsync(
        string id,
        GetClusterOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        var queryParams = new Dictionary<string, string?>();
        if (options?.IncludeMemories == true)
        {
            queryParams["includeMemories"] = "true";
        }
        return await GetAsync<Cluster>($"{BasePath}/{Uri.EscapeDataString(id)}", queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing cluster.
    /// </summary>
    public virtual async Task<Cluster> UpdateAsync(
        string id,
        UpdateClusterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Cluster>($"{BasePath}/{Uri.EscapeDataString(id)}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a cluster.
    /// </summary>
    public new virtual async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists clusters with optional filters.
    /// </summary>
    public virtual async Task<PaginatedResponse<Cluster>> ListAsync(
        ListClustersRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>();

        if (request != null)
        {
            if (request.Limit != null) queryParams["limit"] = request.Limit.ToString();
            if (request.Page != null) queryParams["page"] = request.Page.ToString();
            if (request.Offset != null) queryParams["offset"] = request.Offset.ToString();
            if (request.SpaceId != null) queryParams["spaceId"] = request.SpaceId;
            if (request.SortBy != null) queryParams["sortBy"] = request.SortBy;
            if (request.SortOrder != null) queryParams["sortOrder"] = request.SortOrder;
            if (request.Scale != null) queryParams["scale"] = request.Scale.ToString()?.ToLowerInvariant();
        }

        return await GetAsync<PaginatedResponse<Cluster>>(BasePath, queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a memory to a cluster.
    /// </summary>
    public virtual async Task AddMemoryAsync(
        string clusterId,
        string memoryId,
        double confidence = 1.0,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(clusterId);
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        await PostAsync($"{BasePath}/{Uri.EscapeDataString(clusterId)}/memories", new { memoryId, confidence }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a memory from a cluster.
    /// </summary>
    public virtual async Task RemoveMemoryAsync(
        string clusterId,
        string memoryId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(clusterId);
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        await base.DeleteAsync($"{BasePath}/{Uri.EscapeDataString(clusterId)}/memories/{Uri.EscapeDataString(memoryId)}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Expands a cluster by finding similar memories.
    /// </summary>
    public virtual async Task<ExpandResult> ExpandAsync(
        string clusterId,
        int? limit = null,
        double? threshold = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(clusterId);
        var queryParams = BuildQueryParams(("limit", limit), ("threshold", threshold));
        return await GetAsync<ExpandResult>($"{BasePath}/{Uri.EscapeDataString(clusterId)}/expand", queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Iterates through all clusters matching the criteria.
    /// </summary>
    public virtual async IAsyncEnumerable<Cluster> ListAllAsync(
        ListClustersRequest? request = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        request ??= new ListClustersRequest();
        request.Limit ??= 100;
        request.Page ??= 1;

        int pages = 0;
        const int MaxPages = 1000;

        while (true)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

            var response = await ListAsync(request, cancellationToken).ConfigureAwait(false);

            foreach (var cluster in response.Data)
            {
                yield return cluster;
            }

            if (response.Pagination?.HasMore != true || response.Data.Count == 0)
            {
                break;
            }

            request.Page++;
        }
    }

    /// <summary>
    /// Creates multiple clusters in bulk.
    /// </summary>
    public virtual async Task<BulkResult> BulkCreateAsync(
        IEnumerable<CreateClusterRequest> clusters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(clusters);
        const int MaxBatchSize = 1000;
        var list = clusters.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");
        return await PostAsync<BulkResult>($"{BasePath}/bulk", new { clusters = list }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates multiple clusters in bulk.
    /// </summary>
    public virtual async Task<BulkResult> BulkUpdateAsync(
        IEnumerable<BulkClusterUpdateItem> updates,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updates);
        const int MaxBatchSize = 1000;
        var list = updates.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");
        return await PatchAsync<BulkResult>($"{BasePath}/bulk", new { updates = list }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes multiple clusters in bulk.
    /// </summary>
    public virtual async Task<BulkResult> BulkDeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        const int MaxBatchSize = 1000;
        var list = ids.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");
        return await PostAsync<BulkResult>($"{BasePath}/bulk-delete", new { ids = list }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets cluster statistics.
    /// </summary>
    public virtual async Task<ClusterStats> GetStatsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<ClusterStats>($"{BasePath}/stats", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers incremental clustering.
    /// </summary>
    public virtual async Task<IncrementalClusterResult> IncrementalClusteringAsync(
        IncrementalClusterRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<IncrementalClusterResult>($"{BasePath}/incremental", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Refreshes quality metrics for a cluster.
    /// </summary>
    public virtual async Task<Cluster> RefreshMetricsAsync(
        string clusterId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(clusterId);
        return await PostAsync<Cluster>($"{BasePath}/{Uri.EscapeDataString(clusterId)}/refresh-metrics", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Recomputes the centroid for a cluster.
    /// </summary>
    public virtual async Task<Cluster> RecomputeCentroidAsync(
        string clusterId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(clusterId);
        return await PostAsync<Cluster>($"{BasePath}/{Uri.EscapeDataString(clusterId)}/recompute-centroid", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets quality metrics for a cluster.
    /// </summary>
    public virtual async Task<ClusterQuality> GetQualityAsync(
        string clusterId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(clusterId);
        return await GetAsync<ClusterQuality>($"{BasePath}/{Uri.EscapeDataString(clusterId)}/quality", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets topics for a cluster.
    /// </summary>
    public virtual async Task<ClusterTopics> GetTopicsAsync(
        string clusterId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(clusterId);
        return await GetAsync<ClusterTopics>($"{BasePath}/{Uri.EscapeDataString(clusterId)}/topics", cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Item for bulk cluster update operation.
/// </summary>
public class BulkClusterUpdateItem
{
    /// <summary>Gets or sets the cluster ID.</summary>
    public required string Id { get; set; }

    /// <summary>Gets or sets the name to update.</summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets the description to update.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets metadata to update.</summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
