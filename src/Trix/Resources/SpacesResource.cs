using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Provides methods for managing spaces (workspaces).
/// </summary>
public class SpacesResource : BaseResource
{
    private const string BasePath = "/v1/spaces";

    internal SpacesResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Creates a new space.
    /// </summary>
    public virtual async Task<Space> CreateAsync(
        CreateSpaceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Space>(BasePath, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new space with the specified name.
    /// </summary>
    public virtual async Task<Space> CreateAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateAsync(new CreateSpaceRequest
        {
            Name = name,
            Description = description
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a space by ID.
    /// </summary>
    public virtual async Task<Space> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<Space>($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a space by slug (URL-friendly identifier).
    /// </summary>
    /// <param name="slug">The space slug.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The space with the specified slug.</returns>
    public virtual async Task<Space> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(slug);
        return await GetAsync<Space>($"{BasePath}/{Uri.EscapeDataString(slug)}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing space.
    /// </summary>
    public virtual async Task<Space> UpdateAsync(
        string id,
        UpdateSpaceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Space>($"{BasePath}/{Uri.EscapeDataString(id)}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a space.
    /// </summary>
    public new virtual async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all spaces.
    /// </summary>
    public virtual async Task<PaginatedResponse<Space>> ListAsync(
        int? limit = null,
        int? page = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(("limit", limit), ("page", page));
        return await GetAsync<PaginatedResponse<Space>>(BasePath, queryParams, cancellationToken).ConfigureAwait(false);
    }
}
