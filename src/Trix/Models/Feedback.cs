using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Feedback type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum FeedbackType
{
    [JsonPropertyName("positive")]
    Positive,
    [JsonPropertyName("negative")]
    Negative,
    [JsonPropertyName("neutral")]
    Neutral
}

/// <summary>
/// Quick feedback type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum QuickFeedbackType
{
    [JsonPropertyName("thumbs_up")]
    ThumbsUp,
    [JsonPropertyName("thumbs_down")]
    ThumbsDown
}

/// <summary>
/// Request to submit feedback.
/// </summary>
public class SubmitFeedbackRequest
{
    /// <summary>
    /// Memory ID to provide feedback on.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Feedback type.
    /// </summary>
    [JsonPropertyName("type")]
    public FeedbackType Type { get; set; }

    /// <summary>
    /// Optional comment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request for quick feedback.
/// </summary>
public class QuickFeedbackRequest
{
    /// <summary>
    /// Memory ID to provide feedback on.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Quick feedback type.
    /// </summary>
    [JsonPropertyName("type")]
    public QuickFeedbackType Type { get; set; }
}

/// <summary>
/// A single feedback item in a batch.
/// </summary>
public class BatchFeedbackItem
{
    /// <summary>
    /// Memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Feedback type.
    /// </summary>
    [JsonPropertyName("type")]
    public FeedbackType Type { get; set; }

    /// <summary>
    /// Optional comment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// Request for batch feedback.
/// </summary>
public class BatchFeedbackRequest
{
    /// <summary>
    /// Feedback items to submit.
    /// </summary>
    [JsonPropertyName("feedback")]
    public List<BatchFeedbackItem> Feedback { get; set; } = new();
}

/// <summary>
/// Result of a feedback submission.
/// </summary>
public class FeedbackResult
{
    /// <summary>
    /// Feedback ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Memory ID.
    /// </summary>
    [JsonPropertyName("memoryId")]
    public string MemoryId { get; set; } = string.Empty;

    /// <summary>
    /// Feedback type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// When the feedback was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Result of batch feedback submission.
/// </summary>
public class BatchFeedbackResult
{
    /// <summary>
    /// Number of successful submissions.
    /// </summary>
    [JsonPropertyName("success")]
    public int Success { get; set; }

    /// <summary>
    /// Number of failed submissions.
    /// </summary>
    [JsonPropertyName("failed")]
    public int Failed { get; set; }

    /// <summary>
    /// Error details for failed items.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<BulkError>? Errors { get; set; }
}
