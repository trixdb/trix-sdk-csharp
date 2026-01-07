using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Provides methods for managing resources (projects, topics, etc.) and their relationships with memories.
/// </summary>
public class ResourcesResource : BaseResource
{
    private const string BasePath = "/v1/resources";

    internal ResourcesResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    public virtual async Task<Resource> CreateAsync(
        CreateResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Resource>(BasePath, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new resource with the specified name.
    /// </summary>
    public virtual async Task<Resource> CreateAsync(
        string name,
        string? type = null,
        string? description = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateAsync(new CreateResourceRequest
        {
            Name = name,
            Type = type,
            Description = description,
            Metadata = metadata
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a resource by ID.
    /// </summary>
    public virtual async Task<Resource> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<Resource>($"{BasePath}/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists resources with optional filtering and pagination.
    /// </summary>
    public virtual async Task<PaginatedResponse<Resource>> ListAsync(
        string? type = null,
        string? search = null,
        int? limit = null,
        int? offset = null,
        string? sort = null,
        string? order = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("type", type),
            ("search", search),
            ("limit", limit),
            ("offset", offset),
            ("sort", sort),
            ("order", order)
        );
        return await GetAsync<PaginatedResponse<Resource>>(BasePath, queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    public virtual async Task<Resource> UpdateAsync(
        string id,
        UpdateResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Resource>($"{BasePath}/{id}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    public new virtual async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"{BasePath}/{id}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets memories linked to a resource.
    /// </summary>
    public virtual async Task<ResourceMemoriesResult> GetMemoriesAsync(
        string id,
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        var queryParams = BuildQueryParams(
            ("limit", limit),
            ("offset", offset)
        );
        return await GetAsync<ResourceMemoriesResult>($"{BasePath}/{id}/memories", queryParams, cancellationToken).ConfigureAwait(false);
    }
}
