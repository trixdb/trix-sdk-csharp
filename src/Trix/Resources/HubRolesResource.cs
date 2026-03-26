using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for managing hub custom roles (ADR-080).
/// </summary>
public class HubRolesResource : BaseResource
{
    internal HubRolesResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Lists all roles in a hub.
    /// </summary>
    public virtual async Task<List<HubRole>> ListAsync(
        string hubId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(hubId);
        return await GetAsync<List<HubRole>>(
            $"/v1/hubs/{Esc(hubId)}/roles",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a custom role in a hub.
    /// </summary>
    public virtual async Task<HubRole> CreateAsync(
        string hubId,
        CreateHubRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(hubId);
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<HubRole>(
            $"/v1/hubs/{Esc(hubId)}/roles",
            request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a role in a hub.
    /// </summary>
    public virtual async Task<HubRole> UpdateAsync(
        string hubId,
        string roleId,
        UpdateHubRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(hubId);
        ArgumentException.ThrowIfNullOrEmpty(roleId);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<HubRole>(
            $"/v1/hubs/{Esc(hubId)}/roles/{Esc(roleId)}",
            request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a role from a hub.
    /// </summary>
    public virtual new async Task DeleteAsync(
        string hubId,
        string roleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(hubId);
        ArgumentException.ThrowIfNullOrEmpty(roleId);
        await base.DeleteAsync(
            $"/v1/hubs/{Esc(hubId)}/roles/{Esc(roleId)}",
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Assigns a role to a hub member.
    /// </summary>
    public virtual async Task AssignAsync(
        string hubId,
        string roleId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(hubId);
        ArgumentException.ThrowIfNullOrEmpty(roleId);
        ArgumentException.ThrowIfNullOrEmpty(userId);
        await PostAsync(
            $"/v1/hubs/{Esc(hubId)}/roles/{Esc(roleId)}/assign",
            new AssignRoleRequest { UserId = userId },
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a role from a hub member.
    /// </summary>
    public virtual async Task RemoveAsync(
        string hubId,
        string roleId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(hubId);
        ArgumentException.ThrowIfNullOrEmpty(roleId);
        ArgumentException.ThrowIfNullOrEmpty(userId);
        await base.DeleteAsync(
            $"/v1/hubs/{Esc(hubId)}/roles/{Esc(roleId)}/members/{Esc(userId)}",
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reorders roles within a hub.
    /// </summary>
    public virtual async Task ReorderAsync(
        string hubId,
        List<string> roleIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(hubId);
        ArgumentNullException.ThrowIfNull(roleIds);
        await PutAsync<object>(
            $"/v1/hubs/{Esc(hubId)}/roles/reorder",
            new { role_ids = roleIds },
            cancellationToken).ConfigureAwait(false);
    }

    private static string Esc(string value) => Uri.EscapeDataString(value);
}
