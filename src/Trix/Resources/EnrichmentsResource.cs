using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for memory enrichment operations.
/// </summary>
public class EnrichmentsResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the EnrichmentsResource.
    /// </summary>
    internal EnrichmentsResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Lists enrichments for a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of enrichments.</returns>
    public virtual async Task<List<Enrichment>> ListAsync(
        string memoryId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        return await GetAsync<List<Enrichment>>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/enrichments", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a specific enrichment for a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="enrichmentType">The enrichment type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The enrichment.</returns>
    public virtual async Task<Enrichment> GetAsync(
        string memoryId,
        EnrichmentType enrichmentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var typeName = enrichmentType.ToWireValue();
        return await GetAsync<Enrichment>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/enrichments/{Uri.EscapeDataString(typeName)}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers enrichment processing for a memory.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="request">Enrichment parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The trigger result.</returns>
    public virtual async Task<TriggerEnrichmentResult> TriggerAsync(
        string memoryId,
        TriggerEnrichmentRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);
        return await PostAsync<TriggerEnrichmentResult>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/enrichments/trigger", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retries a failed enrichment.
    /// </summary>
    /// <param name="memoryId">The memory ID.</param>
    /// <param name="enrichmentType">The enrichment type to retry.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The trigger result.</returns>
    public virtual async Task<TriggerEnrichmentResult> RetryAsync(
        string memoryId,
        EnrichmentType enrichmentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(memoryId);

        var typeName = enrichmentType.ToWireValue();
        return await PostAsync<TriggerEnrichmentResult>($"/v1/memories/{Uri.EscapeDataString(memoryId)}/enrichments/{Uri.EscapeDataString(typeName)}/retry", null, cancellationToken)
            .ConfigureAwait(false);
    }
}
