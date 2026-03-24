using System.Runtime.CompilerServices;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for agent session management and memory consolidation.
/// </summary>
public class AgentResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the AgentResource.
    /// </summary>
    internal AgentResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Creates a new agent session.
    /// </summary>
    /// <param name="request">The session creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created session.</returns>
    public virtual async Task<Session> CreateSessionAsync(
        CreateSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Session>("/v1/agent/sessions", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a session by ID.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="includeMemories">Whether to include session memories.</param>
    /// <param name="limit">Maximum memories to include.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session with optional memory history.</returns>
    public virtual async Task<SessionHistory> GetSessionAsync(
        string sessionId,
        bool includeMemories = false,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);

        var queryParams = BuildQueryParams(
            ("includeMemories", includeMemories),
            ("limit", limit)
        );

        return await GetAsync<SessionHistory>($"/v1/agent/sessions/{Uri.EscapeDataString(sessionId)}", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists sessions.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of sessions.</returns>
    public virtual async Task<PaginatedResponse<Session>> ListSessionsAsync(
        ListSessionsRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("limit", request?.Limit),
            ("page", request?.Page),
            ("spaceId", request?.SpaceId),
            ("active", request?.Active)
        );

        return await GetAsync<PaginatedResponse<Session>>("/v1/agent/sessions", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all sessions with automatic pagination.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of sessions.</returns>
    public virtual async IAsyncEnumerable<Session> ListSessionsAllAsync(
        ListSessionsRequest? request = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;
        var limit = request?.Limit ?? 100;

        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await ListSessionsAsync(new ListSessionsRequest
            {
                Limit = limit,
                Page = page,
                SpaceId = request?.SpaceId,
                Active = request?.Active
            }, cancellationToken).ConfigureAwait(false);

            foreach (var session in response.Data)
            {
                yield return session;
            }

            if (response.Pagination?.HasMore != true)
            {
                break;
            }

            page++;
        }
    }

    /// <summary>
    /// Adds a memory to a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="request">The memory to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session memory.</returns>
    public virtual async Task<SessionMemory> AddSessionMemoryAsync(
        string sessionId,
        AddSessionMemoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<SessionMemory>($"/v1/agent/sessions/{Uri.EscapeDataString(sessionId)}/memories", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets context for an agent.
    /// </summary>
    /// <param name="request">The context request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The agent context.</returns>
    public virtual async Task<AgentContextResult> GetContextAsync(
        AgentContextRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<AgentContextResult>("/v1/agent/context", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Ends a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="request">End session options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The end session result.</returns>
    public virtual async Task<EndSessionResult> EndSessionAsync(
        string sessionId,
        EndSessionRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        return await PostAsync<EndSessionResult>($"/v1/agent/sessions/{Uri.EscapeDataString(sessionId)}/end", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets core memory for a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The core memory.</returns>
    public virtual async Task<CoreMemory> GetCoreMemoryAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        return await GetAsync<CoreMemory>($"/v1/agent/sessions/{Uri.EscapeDataString(sessionId)}/core-memory", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates core memory blocks for a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="blocks">The core memory blocks.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated core memory.</returns>
    public virtual async Task<CoreMemory> UpdateCoreMemoryAsync(
        string sessionId,
        List<CoreMemoryBlock> blocks,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        ArgumentNullException.ThrowIfNull(blocks);

        var request = new { blocks };
        return await PutAsync<CoreMemory>($"/v1/agent/sessions/{Uri.EscapeDataString(sessionId)}/core-memory", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets formatted core memory for prompt injection.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The formatted core memory context.</returns>
    public virtual async Task<CoreMemoryContext> FormatCoreMemoryAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        return await GetAsync<CoreMemoryContext>($"/v1/agent/sessions/{Uri.EscapeDataString(sessionId)}/core-memory/format", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers memory consolidation.
    /// </summary>
    /// <param name="request">Consolidation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The consolidation result.</returns>
    public virtual async Task<ConsolidationResult> ConsolidateAsync(
        ConsolidateRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<ConsolidationResult>("/v1/agent/consolidate", request, cancellationToken)
            .ConfigureAwait(false);
    }
}
