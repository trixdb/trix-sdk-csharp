using System.Text.Json;
using Trix.Internal;

namespace Trix.Resources;

/// <summary>
/// Resource for knowledge synthesis operations.
/// </summary>
public class KnowledgeResource : BaseResource
{
    internal KnowledgeResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Generates a knowledge summary for a topic.
    /// </summary>
    /// <param name="topic">The topic to summarize.</param>
    /// <param name="limit">Maximum number of memories to consider.</param>
    /// <param name="spaceId">Optional space ID to scope the summary.</param>
    /// <param name="groupBy">How to group results (default: "tags").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The knowledge summary.</returns>
    public virtual async Task<JsonElement> SummaryAsync(
        string topic,
        int limit = 30,
        string? spaceId = null,
        string groupBy = "tags",
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(topic);
        var request = new Dictionary<string, object>
        {
            ["topic"] = topic,
            ["limit"] = limit,
            ["group_by"] = groupBy
        };
        if (spaceId != null) request["space_id"] = spaceId;
        return await PostAsync<JsonElement>("/v1/knowledge/summary", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Converts memories into a structured note.
    /// </summary>
    /// <param name="memoryIds">Optional memory IDs to include.</param>
    /// <param name="query">Optional query to select memories.</param>
    /// <param name="title">Optional title for the note.</param>
    /// <param name="spaceId">Optional space ID.</param>
    /// <param name="limit">Maximum number of memories to include.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated note.</returns>
    public virtual async Task<JsonElement> ToNoteAsync(
        string[]? memoryIds = null,
        string? query = null,
        string? title = null,
        string? spaceId = null,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, object> { ["limit"] = limit };
        if (memoryIds != null) request["memory_ids"] = memoryIds;
        if (query != null) request["query"] = query;
        if (title != null) request["title"] = title;
        if (spaceId != null) request["space_id"] = spaceId;
        return await PostAsync<JsonElement>("/v1/knowledge/to-note", request, cancellationToken)
            .ConfigureAwait(false);
    }
}
