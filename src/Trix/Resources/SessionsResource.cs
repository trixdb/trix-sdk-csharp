using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Provides methods for managing CLI sessions.
/// Sessions are first-class resources for managing conversation/project/task contexts with lifecycle management.
/// </summary>
public class SessionsResource : BaseResource
{
    private const string BasePath = "/v1/cli-sessions";

    internal SessionsResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Creates a new session.
    /// </summary>
    /// <param name="request">The session creation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created session.</returns>
    /// <exception cref="ArgumentNullException">If request is null.</exception>
    public virtual async Task<CliSession> CreateAsync(
        CreateCliSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<CliSession>(BasePath, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new session with the specified name.
    /// </summary>
    /// <param name="name">The session name.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created session.</returns>
    public virtual async Task<CliSession> CreateAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateAsync(new CreateCliSessionRequest
        {
            Name = name,
            Description = description
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists sessions with optional filtering and pagination.
    /// </summary>
    /// <param name="parameters">Optional filtering and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of sessions.</returns>
    public virtual async Task<PaginatedResponse<CliSession>> ListAsync(
        ListCliSessionsParams? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>();

        if (parameters != null)
        {
            if (parameters.Status.HasValue)
                queryParams["status"] = parameters.Status.Value.ToString().ToLowerInvariant();

            if (parameters.Type.HasValue)
                queryParams["type"] = parameters.Type.Value.ToString().ToLowerInvariant();

            if (!string.IsNullOrEmpty(parameters.SpaceId))
                queryParams["spaceId"] = parameters.SpaceId;

            if (parameters.Tags != null && parameters.Tags.Count > 0)
                queryParams["tags"] = string.Join(",", parameters.Tags);

            if (!string.IsNullOrEmpty(parameters.Search))
                queryParams["search"] = parameters.Search;

            if (parameters.Limit.HasValue)
                queryParams["limit"] = parameters.Limit.Value.ToString();

            if (parameters.Page.HasValue)
                queryParams["page"] = parameters.Page.Value.ToString();

            if (!string.IsNullOrEmpty(parameters.SortBy))
                queryParams["sortBy"] = parameters.SortBy;

            if (!string.IsNullOrEmpty(parameters.SortOrder))
                queryParams["sortOrder"] = parameters.SortOrder;
        }

        return await GetAsync<PaginatedResponse<CliSession>>(
            BasePath,
            queryParams.Count > 0 ? queryParams : null,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets active sessions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of active sessions.</returns>
    public virtual async Task<PaginatedResponse<CliSession>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<PaginatedResponse<CliSession>>(
            $"{BasePath}/active",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets session statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Session statistics.</returns>
    public virtual async Task<CliSessionStats> GetStatsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<CliSessionStats>(
            $"{BasePath}/stats",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a session by ID.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session.</returns>
    /// <exception cref="ArgumentException">If id is null or empty.</exception>
    public virtual async Task<CliSession> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<CliSession>(
            $"{BasePath}/{id}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing session.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <param name="request">The update parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated session.</returns>
    /// <exception cref="ArgumentException">If id is null or empty.</exception>
    /// <exception cref="ArgumentNullException">If request is null.</exception>
    public virtual async Task<CliSession> UpdateAsync(
        string id,
        UpdateCliSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<CliSession>(
            $"{BasePath}/{id}",
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentException">If id is null or empty.</exception>
    public new virtual async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"{BasePath}/{id}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Pauses an active session.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paused session.</returns>
    /// <exception cref="ArgumentException">If id is null or empty.</exception>
    public virtual async Task<CliSession> PauseAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<CliSession>(
            $"{BasePath}/{id}/pause",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resumes a paused session.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The resumed session.</returns>
    /// <exception cref="ArgumentException">If id is null or empty.</exception>
    public virtual async Task<CliSession> ResumeAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<CliSession>(
            $"{BasePath}/{id}/resume",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Completes a session with an optional summary.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <param name="request">Optional completion parameters including summary.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The completed session.</returns>
    /// <exception cref="ArgumentException">If id is null or empty.</exception>
    public virtual async Task<CliSession> CompleteAsync(
        string id,
        CompleteCliSessionRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<CliSession>(
            $"{BasePath}/{id}/complete",
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists memories associated with a session.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of memories.</returns>
    /// <exception cref="ArgumentException">If id is null or empty.</exception>
    public virtual async Task<PaginatedResponse<Memory>> GetMemoriesAsync(
        string id,
        int? limit = null,
        int? page = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var queryParams = BuildQueryParams(
            ("limit", limit),
            ("page", page));

        return await GetAsync<PaginatedResponse<Memory>>(
            $"{BasePath}/{id}/memories",
            queryParams.Count > 0 ? queryParams : null,
            cancellationToken).ConfigureAwait(false);
    }
}
