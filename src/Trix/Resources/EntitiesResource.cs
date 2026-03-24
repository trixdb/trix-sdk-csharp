using System.Runtime.CompilerServices;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for named entity management.
/// </summary>
public class EntitiesResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the EntitiesResource.
    /// </summary>
    internal EntitiesResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="request">The entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created entity.</returns>
    public virtual async Task<Entity> CreateAsync(
        CreateEntityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Entity>("/v1/entities", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets an entity by ID.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity.</returns>
    public virtual async Task<Entity> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<Entity>($"/v1/entities/{Uri.EscapeDataString(id)}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated entity.</returns>
    public virtual async Task<Entity> UpdateAsync(
        string id,
        UpdateEntityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Entity>($"/v1/entities/{Uri.EscapeDataString(id)}", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual new async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"/v1/entities/{Uri.EscapeDataString(id)}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists entities.
    /// </summary>
    /// <param name="limit">Maximum results.</param>
    /// <param name="offset">Offset for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of entities.</returns>
    public virtual async Task<PaginatedResponse<Entity>> ListAsync(
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("limit", limit),
            ("offset", offset)
        );

        return await GetAsync<PaginatedResponse<Entity>>("/v1/entities", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all entities with automatic pagination.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of entities.</returns>
    public virtual async IAsyncEnumerable<Entity> ListAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var offset = 0;
        const int limit = 100;
        int pages = 0;
        const int MaxPages = 1000;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

            var response = await ListAsync(limit, offset, cancellationToken)
                .ConfigureAwait(false);

            foreach (var entity in response.Data)
            {
                yield return entity;
            }

            if (response.Pagination?.HasMore != true)
            {
                break;
            }

            offset += limit;
        }
    }

    /// <summary>
    /// Searches for entities.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="limit">Maximum results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Scored entities matching the query.</returns>
    public virtual async Task<List<ScoredEntity>> SearchAsync(
        string query,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(query);

        var queryParams = BuildQueryParams(
            ("q", query),
            ("limit", limit)
        );

        return await GetAsync<List<ScoredEntity>>("/v1/entities/search", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Finds entities by type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="limit">Maximum results.</param>
    /// <param name="offset">Offset for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of entities.</returns>
    public virtual async Task<PaginatedResponse<Entity>> FindByTypeAsync(
        string entityType,
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(entityType);

        var queryParams = BuildQueryParams(
            ("limit", limit),
            ("offset", offset)
        );

        return await GetAsync<PaginatedResponse<Entity>>($"/v1/entities/type/{Uri.EscapeDataString(entityType)}", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Finds entities linked to a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of entities.</returns>
    public virtual async Task<List<Entity>> FindByMemoryAsync(
        string memoryId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        return await GetAsync<List<Entity>>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/entities", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Resolves text to an entity.
    /// </summary>
    /// <param name="text">The text to resolve.</param>
    /// <param name="entityType">Optional entity type hint.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resolution result.</returns>
    public virtual async Task<EntityResolutionResult> ResolveAsync(
        string text,
        string? entityType = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        var request = new { text, entityType };
        return await PostAsync<EntityResolutionResult>("/v1/entities/resolve", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Merges two entities.
    /// </summary>
    /// <param name="targetId">The target entity ID (will be kept).</param>
    /// <param name="sourceId">The source entity ID (will be merged and deleted).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merged entity.</returns>
    public virtual async Task<Entity> MergeAsync(
        string targetId,
        string sourceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(targetId);
        ArgumentException.ThrowIfNullOrEmpty(sourceId);

        var request = new { sourceId };
        return await PostAsync<Entity>($"/v1/entities/{Uri.EscapeDataString(targetId)}/merge", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Links an entity to a memory.
    /// </summary>
    /// <param name="entityId">The entity ID.</param>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual async Task LinkToMemoryAsync(
        string entityId,
        string memoryId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(entityId);
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var request = new { memoryId };
        await PostAsync($"/v1/entities/{Uri.EscapeDataString(entityId)}/link", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Unlinks an entity from a memory.
    /// </summary>
    /// <param name="entityId">The entity ID.</param>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual async Task UnlinkFromMemoryAsync(
        string entityId,
        string memoryId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(entityId);
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        await base.DeleteAsync($"/v1/entities/{Uri.EscapeDataString(entityId)}/link/{Uri.EscapeDataString(memoryId)}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Extracts entities from a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="save">Whether to save extracted entities.</param>
    /// <param name="link">Whether to link extracted entities to the memory.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extraction result.</returns>
    public virtual async Task<EntityExtractionResult> ExtractAsync(
        string memoryId,
        bool save = true,
        bool link = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var request = new { save, link };
        return await PostAsync<EntityExtractionResult>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/entities/extract", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets available entity types.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of entity types with counts.</returns>
    public virtual async Task<List<EntityTypeInfo>> GetTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<EntityTypeInfo>>("/v1/entities/types", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets facts about an entity.
    /// </summary>
    /// <param name="entityId">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Facts about the entity.</returns>
    public virtual async Task<List<Fact>> GetFactsAsync(
        string entityId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(entityId);
        return await GetAsync<List<Fact>>($"/v1/entities/{Uri.EscapeDataString(entityId)}/facts", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates multiple entities in bulk.
    /// </summary>
    /// <param name="entities">The entities to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bulk operation result.</returns>
    public virtual async Task<BulkResult> BulkCreateAsync(
        IEnumerable<CreateEntityRequest> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        const int MaxBatchSize = 1000;
        var list = entities.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");

        var request = new { entities = list };
        return await PostAsync<BulkResult>("/v1/entities/bulk", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes multiple entities in bulk.
    /// </summary>
    /// <param name="ids">The entity IDs to delete.</param>
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
        return await PostAsync<BulkResult>("/v1/entities/bulk-delete", request, cancellationToken)
            .ConfigureAwait(false);
    }
}
