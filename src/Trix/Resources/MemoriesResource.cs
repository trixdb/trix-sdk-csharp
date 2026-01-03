using System.Text.Json.Serialization;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Provides methods for managing memories.
/// </summary>
public class MemoriesResource : BaseResource
{
    private const string BasePath = "/v1/memories";

    internal MemoriesResource(HttpPipeline pipeline) : base(pipeline) { }

    /// <summary>
    /// Creates a new memory.
    /// </summary>
    /// <param name="request">The memory creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created memory.</returns>
    public virtual async Task<Memory> CreateAsync(
        CreateMemoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Memory>(BasePath, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a memory by ID.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The memory.</returns>
    public virtual async Task<Memory> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<Memory>($"{BasePath}/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated memory.</returns>
    public virtual async Task<Memory> UpdateAsync(
        string id,
        UpdateMemoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Memory>($"{BasePath}/{id}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public new virtual async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"{BasePath}/{id}", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists memories with optional filters.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of memories.</returns>
    public virtual async Task<PaginatedResponse<Memory>> ListAsync(
        ListMemoriesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>();

        if (request != null)
        {
            if (request.Q != null) queryParams["q"] = request.Q;
            if (request.Mode != null) queryParams["mode"] = request.Mode.ToString()?.ToLowerInvariant();
            if (request.Limit != null) queryParams["limit"] = request.Limit.ToString();
            if (request.Page != null) queryParams["page"] = request.Page.ToString();
            if (request.Offset != null) queryParams["offset"] = request.Offset.ToString();
            if (request.Type != null) queryParams["type"] = request.Type.ToString()?.ToLowerInvariant();
            if (request.SpaceId != null) queryParams["spaceId"] = request.SpaceId;
            if (request.SortBy != null) queryParams["sortBy"] = request.SortBy;
            if (request.SortOrder != null) queryParams["sortOrder"] = request.SortOrder;
            if (request.Tags != null && request.Tags.Count > 0)
            {
                queryParams["tags"] = string.Join(",", request.Tags);
            }
        }

        return await GetAsync<PaginatedResponse<Memory>>(BasePath, queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates multiple memories in bulk.
    /// </summary>
    /// <param name="requests">The memory creation requests.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created memories.</returns>
    public virtual async Task<BulkResult> BulkCreateAsync(
        IEnumerable<CreateMemoryRequest> requests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);
        return await PostAsync<BulkResult>($"{BasePath}/bulk", new { memories = requests.ToList() }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates multiple memories in bulk.
    /// </summary>
    /// <param name="updates">The updates to apply (id and fields to update).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bulk operation result.</returns>
    public virtual async Task<BulkResult> BulkUpdateAsync(
        IEnumerable<BulkUpdateItem> updates,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updates);
        return await PostAsync<BulkResult>($"{BasePath}/bulk-update", new { updates = updates.ToList() }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes multiple memories in bulk.
    /// </summary>
    /// <param name="ids">The memory IDs to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bulk operation result.</returns>
    public virtual async Task<BulkResult> BulkDeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return await PostAsync<BulkResult>($"{BasePath}/bulk-delete", new { ids = ids.ToList() }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the audio stream for a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Audio stream.</returns>
    public virtual async Task<Stream> GetAudioAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetStreamAsync($"{BasePath}/{id}/audio", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the transcript for an audio memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transcript result.</returns>
    public virtual async Task<TranscriptResult> GetTranscriptAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<TranscriptResult>($"{BasePath}/{id}/transcript", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Transcribes an audio or video memory with advanced options.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="request">Transcription options (language, provider, diarization, entity detection, etc.).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transcript result.</returns>
    /// <example>
    /// <code>
    /// // Basic transcription
    /// var result = await client.Memories.TranscribeAsync("mem_123");
    ///
    /// // Advanced transcription with speaker diarization
    /// var result = await client.Memories.TranscribeAsync("mem_123", new TranscribeRequest
    /// {
    ///     Provider = "assemblyai",
    ///     EnableSpeakerDiarization = true,
    ///     SpeakersExpected = 2,
    ///     EnableAutoChapters = true,
    ///     EnableEntityDetection = true
    /// });
    /// </code>
    /// </example>
    public virtual async Task<TranscriptResult> TranscribeAsync(
        string id,
        TranscribeRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<TranscriptResult>($"{BasePath}/{id}/transcribe", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a memory from audio or video data.
    ///
    /// Supported audio formats: mp3, mp4, m4a, wav, webm, ogg, flac, aac
    /// Supported video formats: mp4, webm, mov, avi, mkv, flv, mpeg
    ///
    /// For advanced transcription options (speaker diarization, entity detection,
    /// chapters, etc.), use TranscribeAsync() after upload.
    /// </summary>
    /// <param name="audioData">The audio or video data stream.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="contentType">The content type (e.g., 'audio/mpeg', 'video/mp4').</param>
    /// <param name="metadata">Optional metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created memory.</returns>
    /// <example>
    /// <code>
    /// // Upload audio file
    /// using var audioStream = File.OpenRead("recording.mp3");
    /// var memory = await client.Memories.CreateFromAudioAsync(
    ///     audioStream,
    ///     "recording.mp3",
    ///     "audio/mpeg"
    /// );
    ///
    /// // Upload video file
    /// using var videoStream = File.OpenRead("meeting.mp4");
    /// var memory = await client.Memories.CreateFromAudioAsync(
    ///     videoStream,
    ///     "meeting.mp4",
    ///     "video/mp4"
    /// );
    /// </code>
    /// </example>
    public virtual async Task<Memory> CreateFromAudioAsync(
        Stream audioData,
        string fileName,
        string contentType = "audio/mpeg",
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioData);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        return await PostMultipartAsync<Memory>(
            $"{BasePath}/audio",
            audioData,
            fileName,
            contentType,
            metadata,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Iterates through all memories matching the criteria.
    /// </summary>
    /// <param name="request">Optional filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of memories.</returns>
    public virtual async IAsyncEnumerable<Memory> ListAllAsync(
        ListMemoriesRequest? request = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        request ??= new ListMemoriesRequest();
        request.Limit ??= 100;
        request.Page ??= 1;

        while (true)
        {
            var response = await ListAsync(request, cancellationToken).ConfigureAwait(false);

            foreach (var memory in response.Data)
            {
                yield return memory;
            }

            if (response.Pagination?.HasMore != true || response.Data.Count == 0)
            {
                break;
            }

            request.Page++;
        }
    }

    /// <summary>
    /// Gets the memory system configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Memory configuration.</returns>
    public virtual async Task<MemoryConfig> GetConfigAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<MemoryConfig>($"{BasePath}/config", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets memory statistics.
    /// </summary>
    /// <param name="request">Statistics parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Memory statistics.</returns>
    public virtual async Task<MemoryStats> GetStatsAsync(
        GetMemoryStatsRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>();

        if (request != null)
        {
            if (request.SpaceId != null) queryParams["spaceId"] = request.SpaceId;
            if (request.CreatedAfter != null) queryParams["createdAfter"] = request.CreatedAfter.Value.ToString("O");
            if (request.CreatedBefore != null) queryParams["createdBefore"] = request.CreatedBefore.Value.ToString("O");
            if (request.IncludeTypeDistribution != null) queryParams["includeTypeDistribution"] = request.IncludeTypeDistribution.Value.ToString().ToLowerInvariant();
            if (request.IncludeTagDistribution != null) queryParams["includeTagDistribution"] = request.IncludeTagDistribution.Value.ToString().ToLowerInvariant();
            if (request.IncludeTimeline != null) queryParams["includeTimeline"] = request.IncludeTimeline.Value.ToString().ToLowerInvariant();
            if (request.TimelineGranularity != null) queryParams["timelineGranularity"] = request.TimelineGranularity;
        }

        return await GetAsync<MemoryStats>($"{BasePath}/stats", queryParams, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Item for bulk update operation.
/// </summary>
public class BulkUpdateItem
{
    /// <summary>Gets or sets the memory ID.</summary>
    public required string Id { get; set; }

    /// <summary>Gets or sets the content to update.</summary>
    public string? Content { get; set; }

    /// <summary>Gets or sets the metadata to update.</summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Gets or sets tags to update.</summary>
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Result of transcription.
/// </summary>
public class TranscriptResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>Gets or sets the transcript text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the duration in seconds.</summary>
    public double? Duration { get; set; }

    /// <summary>Gets or sets the language detected.</summary>
    public string? Language { get; set; }

    /// <summary>Gets or sets word-level timestamps.</summary>
    public List<WordTimestamp>? Words { get; set; }
}

/// <summary>
/// Word with timestamp.
/// </summary>
public class WordTimestamp
{
    /// <summary>Gets or sets the word.</summary>
    public string Word { get; set; } = string.Empty;

    /// <summary>Gets or sets the start time in seconds.</summary>
    public double Start { get; set; }

    /// <summary>Gets or sets the end time in seconds.</summary>
    public double End { get; set; }

    /// <summary>Gets or sets the confidence score.</summary>
    public double? Confidence { get; set; }
}

/// <summary>
/// Request for transcription with advanced options.
/// </summary>
public class TranscribeRequest
{
    /// <summary>Gets or sets the language code for transcription (e.g., 'en', 'es').</summary>
    public string? Language { get; set; }

    /// <summary>Gets or sets the context to improve transcription accuracy.</summary>
    public string? Prompt { get; set; }

    /// <summary>Gets or sets the transcription provider ('assemblyai' or 'openai').</summary>
    public string? Provider { get; set; }

    /// <summary>Gets or sets whether to enable speaker identification and labeling (diarization).</summary>
    public bool? EnableSpeakerDiarization { get; set; }

    /// <summary>Gets or sets whether to enable entity detection in transcript.</summary>
    public bool? EnableEntityDetection { get; set; }

    /// <summary>Gets or sets whether to enable content safety labeling.</summary>
    public bool? EnableContentSafety { get; set; }

    /// <summary>Gets or sets whether to enable automatic chapter generation.</summary>
    public bool? EnableAutoChapters { get; set; }

    /// <summary>Gets or sets whether to enable automatic summarization.</summary>
    public bool? EnableAutoSummarization { get; set; }

    /// <summary>Gets or sets the expected number of speakers (hint for diarization).</summary>
    public int? SpeakersExpected { get; set; }
}
