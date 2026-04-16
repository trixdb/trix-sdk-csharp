using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Provides methods for managing relationships between memories.
/// </summary>
public class RelationshipsResource : BaseResource
{
    private const string BasePath = "/v1/relationships";

    internal RelationshipsResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Creates a new relationship from a source memory (ADR-145 wire contract).
    /// POSTs to /v1/relationships/{sourceId} with the request in the body.
    /// </summary>
    public virtual async Task<Relationship> CreateAsync(
        string sourceId,
        CreateRelationshipRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sourceId);
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Relationship>(
            $"{BasePath}/{Uri.EscapeDataString(sourceId)}",
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a relationship between two memories.
    /// </summary>
    public virtual async Task<Relationship> CreateAsync(
        string sourceId,
        string targetId,
        string relationshipType,
        double weight = 1.0,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateAsync(sourceId, new CreateRelationshipRequest
        {
            TargetId = targetId,
            RelationshipType = relationshipType,
            Weight = weight,
            Metadata = metadata
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a relationship by ID.
    /// </summary>
    public virtual async Task<Relationship> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<Relationship>($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing relationship.
    /// </summary>
    public virtual async Task<Relationship> UpdateAsync(
        string id,
        UpdateRelationshipRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Relationship>($"{BasePath}/{Uri.EscapeDataString(id)}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a relationship.
    /// </summary>
    public new virtual async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets incoming relationships for a memory.
    /// </summary>
    public virtual async Task<PaginatedResponse<Relationship>> GetIncomingAsync(
        string memoryId,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        var queryParams = BuildQueryParams(("limit", limit));
        return await GetAsync<PaginatedResponse<Relationship>>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/relationships/incoming", queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets outgoing relationships for a memory.
    /// </summary>
    public virtual async Task<PaginatedResponse<Relationship>> GetOutgoingAsync(
        string memoryId,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        var queryParams = BuildQueryParams(("limit", limit));
        return await GetAsync<PaginatedResponse<Relationship>>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/relationships/outgoing", queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reinforces a relationship, increasing its weight.
    /// </summary>
    public virtual async Task<Relationship> ReinforceAsync(
        string id,
        double amount = 0.1,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<Relationship>($"{BasePath}/{Uri.EscapeDataString(id)}/reinforce", new { amount }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Weakens a relationship, decreasing its weight.
    /// </summary>
    public virtual async Task<Relationship> WeakenAsync(
        string id,
        double amount = 0.1,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<Relationship>($"{BasePath}/{Uri.EscapeDataString(id)}/weaken", new { amount }, cancellationToken).ConfigureAwait(false);
    }
}
