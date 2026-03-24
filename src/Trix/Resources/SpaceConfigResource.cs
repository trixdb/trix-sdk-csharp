using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for managing per-space configuration.
/// Provides access to the ADR-042 Space Configuration Layer for
/// memory, LLM, retrieval, and privacy settings.
/// </summary>
public class SpaceConfigResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the SpaceConfigResource.
    /// </summary>
    internal SpaceConfigResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Gets the full configuration for a space.
    /// Returns all categories with their defaults, overrides, and effective values.
    /// </summary>
    /// <param name="spaceId">The space ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The space configuration with version number.</returns>
    public virtual async Task<SpaceConfigResponse> GetAsync(
        string spaceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(spaceId);
        return await GetAsync<SpaceConfigResponse>(
            $"/v1/spaces/{Uri.EscapeDataString(spaceId)}/config",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates space configuration using JSON Merge Patch semantics.
    /// Only include the categories and keys you want to change.
    /// Set a key to null to remove an override and revert to the default.
    /// </summary>
    /// <param name="spaceId">The space ID.</param>
    /// <param name="patch">The configuration patch to apply.</param>
    /// <param name="expectedVersion">Optional version for optimistic concurrency (sent as If-Match header).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated space configuration.</returns>
    public virtual async Task<SpaceConfigResponse> UpdateAsync(
        string spaceId,
        SpaceConfigPatch patch,
        int? expectedVersion = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(spaceId);
        ArgumentNullException.ThrowIfNull(patch);
        Dictionary<string, string>? headers = null;
        if (expectedVersion.HasValue)
        {
            headers = new Dictionary<string, string>
            {
                ["If-Match"] = expectedVersion.Value.ToString()
            };
        }
        return await PatchAsync<SpaceConfigResponse>(
            $"/v1/spaces/{Uri.EscapeDataString(spaceId)}/config",
            patch,
            headers,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates a configuration patch without applying it (dry run).
    /// </summary>
    /// <param name="spaceId">The space ID.</param>
    /// <param name="patch">The configuration patch to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The validation result with errors (if any) and affected categories.</returns>
    public virtual async Task<SpaceConfigValidation> ValidateAsync(
        string spaceId,
        SpaceConfigPatch patch,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(spaceId);
        ArgumentNullException.ThrowIfNull(patch);
        return await PostAsync<SpaceConfigValidation>(
            $"/v1/spaces/{Uri.EscapeDataString(spaceId)}/config/validate",
            patch,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the configuration audit trail for a space.
    /// Returns a paginated list of all configuration changes.
    /// </summary>
    /// <param name="spaceId">The space ID.</param>
    /// <param name="limit">Maximum number of events to return.</param>
    /// <param name="offset">Number of events to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated audit events.</returns>
    public virtual async Task<SpaceConfigAuditResponse> AuditAsync(
        string spaceId,
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(spaceId);
        var queryParams = BuildQueryParams(
            ("limit", limit),
            ("offset", offset));
        return await GetAsync<SpaceConfigAuditResponse>(
            $"/v1/spaces/{Uri.EscapeDataString(spaceId)}/config/audit",
            queryParams,
            cancellationToken).ConfigureAwait(false);
    }
}
