using System.Text.Json;
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
        return await GetAsync<Memory>($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken: cancellationToken).ConfigureAwait(false);
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
        return await PatchAsync<Memory>($"{BasePath}/{Uri.EscapeDataString(id)}", request, cancellationToken).ConfigureAwait(false);
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
        await base.DeleteAsync($"{BasePath}/{Uri.EscapeDataString(id)}", cancellationToken).ConfigureAwait(false);
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
            if (request.Mode != null) queryParams["mode"] = request.Mode.Value.ToWireValue();
            if (request.Limit != null) queryParams["limit"] = request.Limit.ToString();
            if (request.Page != null) queryParams["page"] = request.Page.ToString();
            if (request.Offset != null) queryParams["offset"] = request.Offset.ToString();
            if (request.Type != null) queryParams["type"] = request.Type.Value.ToWireValue();
            if (request.SpaceId != null) queryParams["spaceId"] = request.SpaceId;
            if (request.SortBy != null) queryParams["sortBy"] = request.SortBy;
            if (request.SortOrder != null) queryParams["sortOrder"] = request.SortOrder;
            if (request.Tags != null && request.Tags.Count > 0)
            {
                queryParams["tags"] = string.Join(",", request.Tags);
            }
            if (request.Pinned != null) queryParams["pinned"] = request.Pinned.Value.ToString().ToLowerInvariant();
            if (request.Protected != null) queryParams["protected"] = request.Protected.Value.ToString().ToLowerInvariant();
            if (request.MinQuality != null) queryParams["minQuality"] = request.MinQuality.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (request.IncludeDeleted != null) queryParams["includeDeleted"] = request.IncludeDeleted.Value.ToString().ToLowerInvariant();
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
        const int MaxBatchSize = 1000;
        var list = requests.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");
        return await PostAsync<BulkResult>($"{BasePath}/bulk", new { memories = list }, cancellationToken).ConfigureAwait(false);
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
        const int MaxBatchSize = 1000;
        var list = updates.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");
        return await PostAsync<BulkResult>($"{BasePath}/bulk-update", new { updates = list }, cancellationToken).ConfigureAwait(false);
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
        const int MaxBatchSize = 1000;
        var list = ids.ToList();
        if (list.Count > MaxBatchSize)
            throw new ArgumentException($"Batch size {list.Count} exceeds maximum {MaxBatchSize}");
        return await PostAsync<BulkResult>($"{BasePath}/bulk-delete", new { ids = list }, cancellationToken).ConfigureAwait(false);
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
        return await GetStreamAsync($"{BasePath}/{Uri.EscapeDataString(id)}/audio", cancellationToken).ConfigureAwait(false);
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
        return await GetAsync<TranscriptResult>($"{BasePath}/{Uri.EscapeDataString(id)}/transcript", cancellationToken: cancellationToken).ConfigureAwait(false);
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
        return await PostAsync<TranscriptResult>($"{BasePath}/{Uri.EscapeDataString(id)}/transcribe", request, cancellationToken).ConfigureAwait(false);
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

        int pages = 0;
        const int MaxPages = 1000;

        while (true)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

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
    /// Pins a memory to prevent it from being automatically cleaned up.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated memory.</returns>
    public virtual async Task<Memory> PinAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<Memory>($"{BasePath}/{Uri.EscapeDataString(id)}/pin", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Unpins a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated memory.</returns>
    public virtual async Task<Memory> UnpinAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<Memory>($"{BasePath}/{Uri.EscapeDataString(id)}/unpin", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the protection level for a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="level">The protection level to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated memory.</returns>
    public virtual async Task<Memory> SetProtectionLevelAsync(
        string id,
        ProtectionLevel level,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<Memory>($"{BasePath}/{Uri.EscapeDataString(id)}/protection", new { level = level.ToWireValue() }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Soft deletes a memory (marks it as deleted but retains the data).
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated memory.</returns>
    public virtual async Task<Memory> SoftDeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<Memory>($"{BasePath}/{Uri.EscapeDataString(id)}/soft-delete", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Restores a soft-deleted memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The restored memory.</returns>
    public virtual async Task<Memory> RestoreAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<Memory>($"{BasePath}/{Uri.EscapeDataString(id)}/restore", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the quality score for a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The quality score (0-1).</returns>
    public virtual async Task<double> GetQualityScoreAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        var result = await GetAsync<QualityScoreResponse>($"{BasePath}/{Uri.EscapeDataString(id)}/quality", cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.Score;
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

    /// <summary>
    /// Gets topics extracted from a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="options">Options for retrieving topics.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of topics extracted from the memory.</returns>
    public virtual async Task<List<Topic>> GetTopicsAsync(
        string id,
        GetTopicsOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var queryParams = new Dictionary<string, string?>();
        if (options != null)
        {
            if (options.Refresh) queryParams["refresh"] = "true";
            if (options.MinRelevance != null) queryParams["minRelevance"] = options.MinRelevance.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (options.Categories != null && options.Categories.Count > 0)
            {
                queryParams["categories"] = string.Join(",", options.Categories);
            }
        }

        return await GetAsync<List<Topic>>($"{BasePath}/{Uri.EscapeDataString(id)}/topics", queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for memories by topic.
    /// </summary>
    /// <param name="topic">The topic to search for.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of memories matching the topic.</returns>
    public virtual async Task<List<Memory>> SearchByTopicAsync(
        string topic,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(topic);

        var queryParams = new Dictionary<string, string?>
        {
            ["topic"] = topic,
            ["limit"] = limit.ToString()
        };

        var response = await GetAsync<PaginatedResponse<Memory>>($"{BasePath}/search/topic", queryParams, cancellationToken).ConfigureAwait(false);
        return response.Data;
    }

    /// <summary>
    /// Enriches a memory with additional extracted information.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="options">Enrichment options specifying which operations to perform.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The enrichment result containing extracted information.</returns>
    public virtual async Task<EnrichmentResult> EnrichAsync(
        string id,
        EnrichMemoryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await PostAsync<EnrichmentResult>($"{BasePath}/{Uri.EscapeDataString(id)}/enrich", options, cancellationToken).ConfigureAwait(false);
    }

    #region Image Methods

    /// <summary>
    /// Creates a memory from image data.
    ///
    /// Supported image formats: jpg, jpeg, png, gif, webp, bmp, tiff
    /// </summary>
    /// <param name="imageData">The image data stream.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="contentType">The content type (e.g., 'image/jpeg', 'image/png').</param>
    /// <param name="request">Optional creation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created memory.</returns>
    /// <example>
    /// <code>
    /// // Upload image from file
    /// using var imageStream = File.OpenRead("photo.jpg");
    /// var memory = await client.Memories.CreateFromImageAsync(
    ///     imageStream,
    ///     "photo.jpg",
    ///     "image/jpeg"
    /// );
    ///
    /// // Upload with auto-tagging
    /// var memory = await client.Memories.CreateFromImageAsync(
    ///     imageStream,
    ///     "photo.jpg",
    ///     "image/jpeg",
    ///     new CreateImageMemoryRequest { AutoTag = true }
    /// );
    /// </code>
    /// </example>
    public virtual async Task<Memory> CreateFromImageAsync(
        Stream imageData,
        string fileName,
        string contentType = "image/jpeg",
        CreateImageMemoryRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageData);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        var additionalFields = new Dictionary<string, object>();
        if (request != null)
        {
            if (request.Tags != null) additionalFields["tags"] = request.Tags;
            if (request.Metadata != null) additionalFields["metadata"] = request.Metadata;
            if (request.SpaceId != null) additionalFields["spaceId"] = request.SpaceId;
            if (request.AutoTag != null) additionalFields["autoTag"] = request.AutoTag;
            if (request.ExtractText != null) additionalFields["extractText"] = request.ExtractText;
            if (request.Description != null) additionalFields["description"] = request.Description;
        }

        return await PostMultipartAsync<Memory>(
            BasePath,
            imageData,
            fileName,
            contentType,
            additionalFields.Count > 0 ? additionalFields : null,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a memory from image data provided as byte array.
    /// </summary>
    /// <param name="imageData">The image data.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="contentType">The content type (e.g., 'image/jpeg', 'image/png').</param>
    /// <param name="request">Optional creation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created memory.</returns>
    public virtual async Task<Memory> CreateFromImageAsync(
        byte[] imageData,
        string fileName,
        string contentType = "image/jpeg",
        CreateImageMemoryRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageData);
        using var stream = new MemoryStream(imageData);
        return await CreateFromImageAsync(stream, fileName, contentType, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the original image for a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The image data as a stream.</returns>
    /// <example>
    /// <code>
    /// using var imageStream = await client.Memories.GetImageAsync("mem_123");
    /// using var fileStream = File.Create("downloaded.jpg");
    /// await imageStream.CopyToAsync(fileStream);
    /// </code>
    /// </example>
    public virtual async Task<Stream> GetImageAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetStreamAsync($"{BasePath}/{Uri.EscapeDataString(id)}/image", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the original image for a memory as a byte array.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The image data as bytes.</returns>
    public virtual async Task<byte[]> GetImageBytesAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        using var stream = await GetImageAsync(id, cancellationToken).ConfigureAwait(false);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Gets the thumbnail for a memory.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The thumbnail data as a stream.</returns>
    public virtual async Task<Stream> GetThumbnailAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetStreamAsync($"{BasePath}/{Uri.EscapeDataString(id)}/thumbnail", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the thumbnail for a memory as a byte array.
    /// </summary>
    /// <param name="id">The memory ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The thumbnail data as bytes.</returns>
    public virtual async Task<byte[]> GetThumbnailBytesAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        using var stream = await GetThumbnailAsync(id, cancellationToken).ConfigureAwait(false);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Searches for visually similar images using an uploaded image.
    /// </summary>
    /// <param name="imageData">The query image data.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="request">Optional search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Visual search results.</returns>
    /// <example>
    /// <code>
    /// using var queryImage = File.OpenRead("query.jpg");
    /// var results = await client.Memories.SearchVisualAsync(
    ///     queryImage,
    ///     "query.jpg",
    ///     "image/jpeg",
    ///     new VisualSearchRequest { Limit = 10, Threshold = 0.7 }
    /// );
    /// foreach (var match in results.Results)
    /// {
    ///     Console.WriteLine($"Found: {match.Memory.Id} (similarity: {match.Similarity})");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<VisualSearchResult> SearchVisualAsync(
        Stream imageData,
        string fileName,
        string contentType = "image/jpeg",
        VisualSearchRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageData);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        var additionalFields = new Dictionary<string, object>();
        if (request != null)
        {
            if (request.Limit != null) additionalFields["limit"] = request.Limit;
            if (request.Threshold != null) additionalFields["threshold"] = request.Threshold;
            if (request.SpaceId != null) additionalFields["spaceId"] = request.SpaceId;
            if (request.IncludeEmbedding != null) additionalFields["includeEmbedding"] = request.IncludeEmbedding;
        }

        return await PostMultipartAsync<VisualSearchResult>(
            $"{BasePath}/search/visual",
            imageData,
            fileName,
            contentType,
            additionalFields.Count > 0 ? additionalFields : null,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for visually similar images using a byte array.
    /// </summary>
    /// <param name="imageData">The query image data.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="request">Optional search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Visual search results.</returns>
    public virtual async Task<VisualSearchResult> SearchVisualAsync(
        byte[] imageData,
        string fileName,
        string contentType = "image/jpeg",
        VisualSearchRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageData);
        using var stream = new MemoryStream(imageData);
        return await SearchVisualAsync(stream, fileName, contentType, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for images using a text query (text-to-image search).
    /// Uses multi-modal embeddings to find images matching the text description.
    /// </summary>
    /// <param name="query">The text query describing the desired images.</param>
    /// <param name="request">Optional search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Visual search results.</returns>
    /// <example>
    /// <code>
    /// var results = await client.Memories.SearchByTextAsync(
    ///     "sunset over the ocean",
    ///     new TextToImageSearchRequest { Limit = 10 }
    /// );
    /// </code>
    /// </example>
    public virtual async Task<VisualSearchResult> SearchByTextAsync(
        string query,
        TextToImageSearchRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(query);

        var body = new Dictionary<string, object> { ["query"] = query };
        if (request != null)
        {
            if (request.Limit != null) body["limit"] = request.Limit;
            if (request.Threshold != null) body["threshold"] = request.Threshold;
            if (request.SpaceId != null) body["spaceId"] = request.SpaceId;
            if (request.Tags != null) body["tags"] = request.Tags;
        }

        return await PostAsync<VisualSearchResult>($"{BasePath}/search/text", body, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Finds visually similar images to an existing memory.
    /// </summary>
    /// <param name="id">The memory ID to find similar images for.</param>
    /// <param name="request">Optional search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Similar memories with similarity scores.</returns>
    /// <example>
    /// <code>
    /// var similar = await client.Memories.FindSimilarAsync(
    ///     "mem_123",
    ///     new FindSimilarImagesRequest { Type = SimilarityType.Image, Limit = 5 }
    /// );
    /// </code>
    /// </example>
    public virtual async Task<SimilarityResult> FindSimilarAsync(
        string id,
        FindSimilarImagesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var queryParams = new Dictionary<string, string?>();
        if (request != null)
        {
            if (request.Type != null) queryParams["type"] = request.Type.Value.ToWireValue();
            if (request.Limit != null) queryParams["limit"] = request.Limit.ToString();
            if (request.Threshold != null) queryParams["threshold"] = request.Threshold.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (request.SpaceId != null) queryParams["spaceId"] = request.SpaceId;
            if (request.IncludeEmbedding != null) queryParams["includeEmbedding"] = request.IncludeEmbedding.Value.ToString().ToLowerInvariant();
        }

        return await GetAsync<SimilarityResult>($"{BasePath}/{Uri.EscapeDataString(id)}/similar", queryParams, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Checks for duplicate images by uploading an image.
    /// </summary>
    /// <param name="imageData">The image data to check.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="request">Optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Duplicate check result.</returns>
    /// <example>
    /// <code>
    /// using var imageStream = File.OpenRead("photo.jpg");
    /// var result = await client.Memories.CheckDuplicatesAsync(
    ///     imageStream,
    ///     "photo.jpg",
    ///     "image/jpeg"
    /// );
    /// if (result.HasDuplicates)
    /// {
    ///     Console.WriteLine($"Found {result.Duplicates.Count} duplicates");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<DuplicateCheckResult> CheckDuplicatesAsync(
        Stream imageData,
        string fileName,
        string contentType = "image/jpeg",
        CheckDuplicatesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageData);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        var additionalFields = new Dictionary<string, object>();
        if (request != null)
        {
            if (request.Threshold != null) additionalFields["threshold"] = request.Threshold;
            if (request.SpaceId != null) additionalFields["spaceId"] = request.SpaceId;
            if (request.Limit != null) additionalFields["limit"] = request.Limit;
        }

        return await PostMultipartAsync<DuplicateCheckResult>(
            $"{BasePath}/check-duplicates",
            imageData,
            fileName,
            contentType,
            additionalFields.Count > 0 ? additionalFields : null,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Checks for duplicate images using a byte array.
    /// </summary>
    /// <param name="imageData">The image data to check.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="request">Optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Duplicate check result.</returns>
    public virtual async Task<DuplicateCheckResult> CheckDuplicatesAsync(
        byte[] imageData,
        string fileName,
        string contentType = "image/jpeg",
        CheckDuplicatesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageData);
        using var stream = new MemoryStream(imageData);
        return await CheckDuplicatesAsync(stream, fileName, contentType, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Checks for duplicates of an existing memory.
    /// </summary>
    /// <param name="id">The memory ID to check duplicates for.</param>
    /// <param name="request">Optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Duplicate check result.</returns>
    public virtual async Task<DuplicateCheckResult> CheckDuplicatesAsync(
        string id,
        CheckDuplicatesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var body = new Dictionary<string, object>();
        if (request != null)
        {
            if (request.Threshold != null) body["threshold"] = request.Threshold;
            if (request.SpaceId != null) body["spaceId"] = request.SpaceId;
            if (request.Limit != null) body["limit"] = request.Limit;
        }

        return await PostAsync<DuplicateCheckResult>($"{BasePath}/{Uri.EscapeDataString(id)}/check-duplicates", body.Count > 0 ? body : null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Clusters images by visual similarity.
    /// </summary>
    /// <param name="request">Optional clustering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cluster result with grouped images.</returns>
    /// <example>
    /// <code>
    /// var result = await client.Memories.ClusterImagesAsync(
    ///     new ClusterImagesRequest { NumClusters = 10 }
    /// );
    /// foreach (var cluster in result.Clusters)
    /// {
    ///     Console.WriteLine($"Cluster {cluster.Id}: {cluster.MemoryIds.Count} images");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<ImageClusterResult> ClusterImagesAsync(
        ClusterImagesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<ImageClusterResult>($"{BasePath}/images/cluster", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Auto-tags an image with detected objects, scenes, and colors.
    /// </summary>
    /// <param name="imageId">The image/memory ID to auto-tag.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Auto-tag result with generated tags.</returns>
    /// <example>
    /// <code>
    /// var result = await client.Memories.AutoTagAsync("mem_123");
    /// foreach (var tag in result.Tags)
    /// {
    ///     Console.WriteLine($"Tag: {tag.Name} (confidence: {tag.Confidence})");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<AutoTagResult> AutoTagAsync(
        string imageId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(imageId);
        return await PostAsync<AutoTagResult>($"{BasePath}/images/{Uri.EscapeDataString(imageId)}/auto-tag", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Auto-tags multiple images in batch.
    /// </summary>
    /// <param name="imageIds">The image/memory IDs to auto-tag.</param>
    /// <param name="request">Optional batch parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Batch auto-tag result.</returns>
    /// <example>
    /// <code>
    /// var result = await client.Memories.BatchAutoTagAsync(
    ///     new[] { "mem_1", "mem_2", "mem_3" },
    ///     new BatchAutoTagRequest { ApplyTags = true, MinConfidence = 0.8 }
    /// );
    /// Console.WriteLine($"Tagged {result.Success} images");
    /// </code>
    /// </example>
    public virtual async Task<BatchAutoTagResult> BatchAutoTagAsync(
        IEnumerable<string> imageIds,
        BatchAutoTagRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageIds);

        var body = new Dictionary<string, object>
        {
            ["imageIds"] = imageIds.ToList()
        };

        if (request != null)
        {
            if (request.ApplyTags != null) body["applyTags"] = request.ApplyTags;
            if (request.MinConfidence != null) body["minConfidence"] = request.MinConfidence;
        }

        return await PostAsync<BatchAutoTagResult>($"{BasePath}/images/batch-auto-tag", body, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets query suggestions based on image content in the space.
    /// </summary>
    /// <param name="request">Optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query suggestions.</returns>
    /// <example>
    /// <code>
    /// var suggestions = await client.Memories.SuggestQueriesAsync(
    ///     new SuggestQueriesRequest { Limit = 10 }
    /// );
    /// foreach (var suggestion in suggestions.Suggestions)
    /// {
    ///     Console.WriteLine($"Try searching: {suggestion.Query}");
    /// }
    /// </code>
    /// </example>
    public virtual async Task<QuerySuggestionsResult> SuggestQueriesAsync(
        SuggestQueriesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>();
        if (request != null)
        {
            if (request.SpaceId != null) queryParams["spaceId"] = request.SpaceId;
            if (request.Limit != null) queryParams["limit"] = request.Limit.ToString();
            if (request.Categories != null && request.Categories.Count > 0)
            {
                queryParams["categories"] = string.Join(",", request.Categories);
            }
        }

        return await GetAsync<QuerySuggestionsResult>($"{BasePath}/images/suggest-queries", queryParams, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Resource Linking

    /// <summary>
    /// Links a resource to a memory.
    /// </summary>
    public virtual async Task<LinkResourceResult> LinkResourceAsync(
        string id,
        string resourceId,
        ResourceRelationshipType? relationshipType = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentException.ThrowIfNullOrEmpty(resourceId);

        var request = new LinkResourceRequest
        {
            ResourceId = resourceId,
            RelationshipType = relationshipType
        };

        return await PostAsync<LinkResourceResult>($"{BasePath}/{Uri.EscapeDataString(id)}/resources", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets resources linked to a memory.
    /// </summary>
    public virtual async Task<MemoryResourcesResult> GetResourcesAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<MemoryResourcesResult>($"{BasePath}/{Uri.EscapeDataString(id)}/resources", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Unlinks a resource from a memory.
    /// </summary>
    public virtual async Task<UnlinkResourceResult> UnlinkResourceAsync(
        string id,
        string resourceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentException.ThrowIfNullOrEmpty(resourceId);
        return await DeleteAsync<UnlinkResourceResult>($"{BasePath}/{Uri.EscapeDataString(id)}/resources/{Uri.EscapeDataString(resourceId)}", cancellationToken).ConfigureAwait(false);
    }

    #endregion

    /// <summary>
    /// Stores a memory and automatically organizes it with tags, metadata, and contradiction detection.
    /// </summary>
    /// <param name="content">The memory content to store.</param>
    /// <param name="tags">Optional tags to apply.</param>
    /// <param name="metadata">Optional metadata dictionary.</param>
    /// <param name="spaceId">Optional space ID.</param>
    /// <param name="detectContradictions">Whether to detect contradictions with existing memories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The store-and-organize result.</returns>
    public virtual async Task<JsonElement> StoreAndOrganizeAsync(
        string content,
        string[]? tags = null,
        Dictionary<string, object>? metadata = null,
        string? spaceId = null,
        bool detectContradictions = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(content);
        var request = new Dictionary<string, object> { ["content"] = content };
        if (tags != null) request["tags"] = tags;
        if (metadata != null) request["metadata"] = metadata;
        if (spaceId != null) request["space_id"] = spaceId;
        if (detectContradictions) request["detect_contradictions"] = true;
        return await PostAsync<JsonElement>("/v1/memories/store-organize", request, cancellationToken)
            .ConfigureAwait(false);
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
/// Result of transcription with full metadata, segments, entities, and chapters.
/// </summary>
public class TranscriptResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>Gets or sets the audio file ID.</summary>
    public string? AudioFileId { get; set; }

    /// <summary>Gets or sets the full transcript text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the duration in seconds.</summary>
    public double? Duration { get; set; }

    /// <summary>Gets or sets the detected language code.</summary>
    public string? Language { get; set; }

    /// <summary>Gets or sets the language detection confidence (0-1).</summary>
    public double? LanguageConfidence { get; set; }

    /// <summary>Gets or sets the transcription provider used ('assemblyai', 'openai').</summary>
    public string? Provider { get; set; }

    /// <summary>Gets or sets the auto-generated summary (when EnableAutoSummarization is true).</summary>
    public string? Summary { get; set; }

    /// <summary>Gets or sets content safety labels (when EnableContentSafety is true).</summary>
    public List<ContentSafetyLabel>? ContentSafetyLabels { get; set; }

    /// <summary>Gets or sets provider-specific metadata.</summary>
    public Dictionary<string, object>? ProviderMetadata { get; set; }

    /// <summary>Gets or sets transcript segments with speaker information (when EnableSpeakerDiarization is true).</summary>
    public List<TranscriptSegment>? Segments { get; set; }

    /// <summary>Gets or sets detected entities (when EnableEntityDetection is true).</summary>
    public List<TranscriptEntity>? Entities { get; set; }

    /// <summary>Gets or sets auto-generated chapters (when EnableAutoChapters is true).</summary>
    public List<TranscriptChapter>? Chapters { get; set; }

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

    /// <summary>Gets or sets the speaker label (when diarization is enabled).</summary>
    public string? Speaker { get; set; }
}

/// <summary>
/// Transcript segment with speaker information and word-level details.
/// </summary>
public class TranscriptSegment
{
    /// <summary>Gets or sets the segment ID.</summary>
    public string? Id { get; set; }

    /// <summary>Gets or sets the start time in seconds.</summary>
    public double StartTime { get; set; }

    /// <summary>Gets or sets the end time in seconds.</summary>
    public double EndTime { get; set; }

    /// <summary>Gets or sets the segment text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the segment index.</summary>
    public int SegmentIndex { get; set; }

    /// <summary>Gets or sets the confidence score (0-1).</summary>
    public double? Confidence { get; set; }

    /// <summary>Gets or sets the speaker label (e.g., 'A', 'B', 'Speaker 1').</summary>
    public string? Speaker { get; set; }

    /// <summary>Gets or sets word-level timestamps for this segment.</summary>
    public List<WordTimestamp>? Words { get; set; }

    /// <summary>Gets or sets the average word confidence.</summary>
    public double? WordConfidenceAvg { get; set; }
}

/// <summary>
/// Entity detected in the transcript.
/// </summary>
public class TranscriptEntity
{
    /// <summary>Gets or sets the entity ID.</summary>
    public string? Id { get; set; }

    /// <summary>Gets or sets the entity type (e.g., 'person_name', 'organization', 'location').</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>Gets or sets the entity text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the start time in seconds.</summary>
    public double? StartTime { get; set; }

    /// <summary>Gets or sets the end time in seconds.</summary>
    public double? EndTime { get; set; }

    /// <summary>Gets or sets the confidence score.</summary>
    public double? Confidence { get; set; }

    /// <summary>Gets or sets additional entity metadata.</summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Auto-generated chapter in the transcript.
/// </summary>
public class TranscriptChapter
{
    /// <summary>Gets or sets the chapter ID.</summary>
    public string? Id { get; set; }

    /// <summary>Gets or sets the chapter index.</summary>
    public int ChapterIndex { get; set; }

    /// <summary>Gets or sets the chapter headline.</summary>
    public string Headline { get; set; } = string.Empty;

    /// <summary>Gets or sets the chapter summary.</summary>
    public string? Summary { get; set; }

    /// <summary>Gets or sets the short gist/title.</summary>
    public string? Gist { get; set; }

    /// <summary>Gets or sets the start time in seconds.</summary>
    public double StartTime { get; set; }

    /// <summary>Gets or sets the end time in seconds.</summary>
    public double EndTime { get; set; }
}

/// <summary>
/// Time range for content safety labels.
/// </summary>
public class TimestampRange
{
    /// <summary>Gets or sets the start time in seconds.</summary>
    public double Start { get; set; }

    /// <summary>Gets or sets the end time in seconds.</summary>
    public double End { get; set; }
}

/// <summary>
/// Content safety label from transcript analysis.
/// </summary>
public class ContentSafetyLabel
{
    /// <summary>Gets or sets the label type (e.g., 'profanity', 'hate_speech').</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the confidence score (0-1).</summary>
    public double Confidence { get; set; }

    /// <summary>Gets or sets the severity level.</summary>
    public string? Severity { get; set; }

    /// <summary>Gets or sets the timestamp range.</summary>
    public TimestampRange? Timestamp { get; set; }
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

/// <summary>
/// Response containing a quality score.
/// </summary>
public class QualityScoreResponse
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>Gets or sets the quality score (0-1).</summary>
    [JsonPropertyName("score")]
    public double Score { get; set; }
}

/// <summary>
/// Represents a topic extracted from a memory.
/// </summary>
public class Topic
{
    /// <summary>Gets or sets the topic name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the relevance score (0-1).</summary>
    [JsonPropertyName("relevance")]
    public double Relevance { get; set; }

    /// <summary>Gets or sets the topic category.</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Options for retrieving topics from a memory.
/// </summary>
public class GetTopicsOptions
{
    /// <summary>Gets or sets whether to refresh the topics (re-extract from content).</summary>
    public bool Refresh { get; set; }

    /// <summary>Gets or sets the minimum relevance threshold (0-1).</summary>
    public double? MinRelevance { get; set; }

    /// <summary>Gets or sets the categories to filter by.</summary>
    public List<string>? Categories { get; set; }
}

/// <summary>
/// Enrichment operation types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum EnrichmentOperation
{
    /// <summary>Extract topics from the memory.</summary>
    [JsonPropertyName("topics")] Topics,

    /// <summary>Generate a summary of the memory.</summary>
    [JsonPropertyName("summary")] Summary,

    /// <summary>Extract named entities from the memory.</summary>
    [JsonPropertyName("entities")] Entities,

    /// <summary>Calculate quality score for the memory.</summary>
    [JsonPropertyName("quality")] Quality
}

/// <summary>
/// Result of enriching a memory.
/// </summary>
public class EnrichmentResult
{
    /// <summary>Gets or sets the memory ID.</summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>Gets or sets the operations that were performed.</summary>
    [JsonPropertyName("operations")]
    public List<EnrichmentOperation> Operations { get; set; } = new();

    /// <summary>Gets or sets the results of the enrichment operations.</summary>
    [JsonPropertyName("results")]
    public Dictionary<string, object> Results { get; set; } = new();
}

/// <summary>
/// Options for enriching a memory.
/// </summary>
public class EnrichMemoryOptions
{
    /// <summary>Gets or sets the enrichment operations to perform. If null, all operations are performed.</summary>
    [JsonPropertyName("operations")]
    public List<EnrichmentOperation>? Operations { get; set; }
}
