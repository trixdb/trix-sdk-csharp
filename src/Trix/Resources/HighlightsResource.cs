using System.Runtime.CompilerServices;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for text highlight management.
/// </summary>
public class HighlightsResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the HighlightsResource.
    /// </summary>
    internal HighlightsResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Creates a new highlight.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="request">The highlight to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created highlight.</returns>
    public virtual async Task<Highlight> CreateAsync(
        string memoryId,
        CreateHighlightRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<Highlight>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/highlights", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a highlight by ID.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="highlightId">The highlight ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The highlight.</returns>
    public virtual async Task<Highlight> GetAsync(
        string memoryId,
        string highlightId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        ArgumentException.ThrowIfNullOrEmpty(highlightId);

        return await GetAsync<Highlight>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/highlights/{Uri.EscapeDataString(highlightId)}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a highlight.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="highlightId">The highlight ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated highlight.</returns>
    public virtual async Task<Highlight> UpdateAsync(
        string memoryId,
        string highlightId,
        UpdateHighlightRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        ArgumentException.ThrowIfNullOrEmpty(highlightId);
        ArgumentNullException.ThrowIfNull(request);

        return await PatchAsync<Highlight>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/highlights/{Uri.EscapeDataString(highlightId)}", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a highlight.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="highlightId">The highlight ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual async Task DeleteAsync(
        string memoryId,
        string highlightId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        ArgumentException.ThrowIfNullOrEmpty(highlightId);

        await base.DeleteAsync($"/v1/memories/{Uri.EscapeDataString(memoryId)}/highlights/{Uri.EscapeDataString(highlightId)}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists highlights for a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of highlights.</returns>
    public virtual async Task<PaginatedResponse<Highlight>> ListAsync(
        string memoryId,
        ListHighlightsRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var queryParams = BuildQueryParams(
            ("limit", request?.Limit),
            ("page", request?.Page)
        );

        return await GetAsync<PaginatedResponse<Highlight>>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/highlights", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all highlights for a memory with automatic pagination.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of highlights.</returns>
    public virtual async IAsyncEnumerable<Highlight> ListAllAsync(
        string memoryId,
        ListHighlightsRequest? request = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var page = 1;
        var limit = request?.Limit ?? 100;
        int pages = 0;
        const int MaxPages = 1000;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

            var response = await ListAsync(memoryId, new ListHighlightsRequest
            {
                Limit = limit,
                Page = page
            }, cancellationToken).ConfigureAwait(false);

            foreach (var highlight in response.Data)
            {
                yield return highlight;
            }

            if (response.Pagination?.HasMore != true)
            {
                break;
            }

            page++;
        }
    }

    /// <summary>
    /// Extracts highlights automatically from a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="request">Extraction parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The extracted highlights.</returns>
    public virtual async Task<ExtractHighlightsResult> ExtractAsync(
        string memoryId,
        ExtractHighlightsRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        return await PostAsync<ExtractHighlightsResult>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/highlights/extract", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all highlights across all memories.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of highlights.</returns>
    public virtual async Task<PaginatedResponse<Highlight>> ListGlobalAsync(
        ListHighlightsRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("limit", request?.Limit),
            ("page", request?.Page)
        );

        return await GetAsync<PaginatedResponse<Highlight>>("/v1/highlights", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Searches highlights semantically.
    /// </summary>
    /// <param name="request">Search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search results.</returns>
    public virtual async Task<HighlightSearchResult> SearchAsync(
        SearchHighlightsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<HighlightSearchResult>("/v1/highlights/search", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets available highlight types.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of highlight types.</returns>
    public virtual async Task<HighlightTypesResult> GetTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<HighlightTypesResult>("/v1/highlights/types", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Links a highlight to a memory.
    /// </summary>
    /// <param name="highlightId">The highlight ID.</param>
    /// <param name="request">Link parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Link result.</returns>
    public virtual async Task<HighlightLinkResult> LinkAsync(
        string highlightId,
        LinkHighlightRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(highlightId);
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<HighlightLinkResult>($"/v1/highlights/{Uri.EscapeDataString(highlightId)}/link", request, cancellationToken)
            .ConfigureAwait(false);
    }
}
