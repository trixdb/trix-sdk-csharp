using System.Runtime.CompilerServices;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for knowledge graph facts management.
/// </summary>
public class FactsResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the FactsResource.
    /// </summary>
    internal FactsResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Creates a new fact.
    /// </summary>
    /// <param name="request">The fact to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created fact.</returns>
    public virtual async Task<Fact> CreateAsync(
        CreateFactRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Fact>("/v1/facts", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a fact by ID.
    /// </summary>
    /// <param name="id">The fact ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The fact.</returns>
    public virtual async Task<Fact> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<Fact>($"/v1/facts/{Uri.EscapeDataString(id)}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a fact.
    /// </summary>
    /// <param name="id">The fact ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated fact.</returns>
    public virtual async Task<Fact> UpdateAsync(
        string id,
        UpdateFactRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Fact>($"/v1/facts/{Uri.EscapeDataString(id)}", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a fact.
    /// </summary>
    /// <param name="id">The fact ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual new async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"/v1/facts/{Uri.EscapeDataString(id)}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists facts with optional filters.
    /// </summary>
    /// <param name="subject">Filter by subject.</param>
    /// <param name="predicate">Filter by predicate.</param>
    /// <param name="obj">Filter by object.</param>
    /// <param name="minConfidence">Minimum confidence threshold.</param>
    /// <param name="spaceId">Filter by space.</param>
    /// <param name="limit">Maximum results.</param>
    /// <param name="page">Page number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of facts.</returns>
    public virtual async Task<PaginatedResponse<Fact>> ListAsync(
        string? subject = null,
        string? predicate = null,
        string? obj = null,
        double? minConfidence = null,
        string? spaceId = null,
        int? limit = null,
        int? page = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("subject", subject),
            ("predicate", predicate),
            ("object", obj),
            ("minConfidence", minConfidence),
            ("spaceId", spaceId),
            ("limit", limit),
            ("page", page)
        );

        return await GetAsync<PaginatedResponse<Fact>>("/v1/facts", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all facts with automatic pagination.
    /// </summary>
    /// <param name="subject">Filter by subject.</param>
    /// <param name="predicate">Filter by predicate.</param>
    /// <param name="obj">Filter by object.</param>
    /// <param name="minConfidence">Minimum confidence threshold.</param>
    /// <param name="spaceId">Filter by space.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of facts.</returns>
    public virtual async IAsyncEnumerable<Fact> ListAllAsync(
        string? subject = null,
        string? predicate = null,
        string? obj = null,
        double? minConfidence = null,
        string? spaceId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;
        const int limit = 100;
        int pages = 0;
        const int MaxPages = 1000;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

            var response = await ListAsync(subject, predicate, obj, minConfidence, spaceId, limit, page, cancellationToken)
                .ConfigureAwait(false);

            foreach (var fact in response.Data)
            {
                yield return fact;
            }

            if (response.Pagination?.HasMore != true)
            {
                break;
            }

            page++;
        }
    }

    /// <summary>
    /// Queries facts using natural language.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <param name="limit">Maximum results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Scored facts matching the query.</returns>
    public virtual async Task<List<ScoredFact>> QueryAsync(
        string query,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(query);

        var queryParams = BuildQueryParams(
            ("q", query),
            ("limit", limit)
        );

        return await GetAsync<List<ScoredFact>>("/v1/facts/query", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Extracts facts from a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="save">Whether to save extracted facts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extraction result with facts.</returns>
    public virtual async Task<FactExtractionResult> ExtractAsync(
        string memoryId,
        bool save = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var request = new { save };
        return await PostAsync<FactExtractionResult>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/facts/extract", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies a fact against supporting evidence.
    /// </summary>
    /// <param name="id">The fact ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Verification result.</returns>
    public virtual async Task<FactVerificationResult> VerifyAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<FactVerificationResult>($"/v1/facts/{Uri.EscapeDataString(id)}/verify", null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates multiple facts in bulk.
    /// </summary>
    /// <param name="facts">The facts to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bulk operation result.</returns>
    public virtual async Task<BulkResult> BulkCreateAsync(
        IEnumerable<CreateFactRequest> facts,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(facts);
        const int MaxBatchSize = 1000;
        var list = facts.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");

        var request = new { facts = list };
        return await PostAsync<BulkResult>("/v1/facts/bulk", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes multiple facts in bulk.
    /// </summary>
    /// <param name="ids">The fact IDs to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bulk operation result.</returns>
    public virtual async Task<BulkResult> BulkDeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        const int MaxBatchSize = 1000;
        var list = ids.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");

        var request = new { ids = list };
        return await PostAsync<BulkResult>("/v1/facts/bulk-delete", request, cancellationToken)
            .ConfigureAwait(false);
    }
}
