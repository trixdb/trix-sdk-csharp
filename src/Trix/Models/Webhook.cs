using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Webhook event types.
/// </summary>
public static class WebhookEvents
{
    public const string MemoryCreated = "memory.created";
    public const string MemoryUpdated = "memory.updated";
    public const string MemoryDeleted = "memory.deleted";
    public const string RelationshipCreated = "relationship.created";
    public const string RelationshipUpdated = "relationship.updated";
    public const string RelationshipDeleted = "relationship.deleted";
    public const string ClusterCreated = "cluster.created";
    public const string ClusterUpdated = "cluster.updated";
    public const string ClusterDeleted = "cluster.deleted";
    public const string SessionCreated = "session.created";
    public const string SessionEnded = "session.ended";
}

/// <summary>
/// A webhook subscription.
/// </summary>
public class Webhook
{
    /// <summary>
    /// Unique identifier for the webhook.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// URL to receive webhook events.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Events this webhook is subscribed to.
    /// </summary>
    [JsonPropertyName("events")]
    public List<string> Events { get; set; } = new();

    /// <summary>
    /// Secret for verifying webhook signatures.
    /// </summary>
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }

    /// <summary>
    /// Whether the webhook is active.
    /// </summary>
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    /// <summary>
    /// Custom headers to include in webhook requests.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// When the webhook was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the webhook was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request to create a webhook.
/// </summary>
public class CreateWebhookRequest
{
    /// <summary>
    /// URL to receive webhook events.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Events to subscribe to.
    /// </summary>
    [JsonPropertyName("events")]
    public List<string> Events { get; set; } = new();

    /// <summary>
    /// Secret for signature verification.
    /// </summary>
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }

    /// <summary>
    /// Whether the webhook is active.
    /// </summary>
    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    /// <summary>
    /// Custom headers to include.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to update a webhook.
/// </summary>
public class UpdateWebhookRequest
{
    /// <summary>
    /// URL to receive webhook events.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Events to subscribe to.
    /// </summary>
    [JsonPropertyName("events")]
    public List<string>? Events { get; set; }

    /// <summary>
    /// Whether the webhook is active.
    /// </summary>
    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    /// <summary>
    /// Custom headers to include.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Webhook delivery status.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DeliveryStatus
{
    [JsonPropertyName("pending")]
    Pending,
    [JsonPropertyName("success")]
    Success,
    [JsonPropertyName("failed")]
    Failed,
    [JsonPropertyName("retrying")]
    Retrying
}

/// <summary>
/// A webhook delivery attempt.
/// </summary>
public class WebhookDelivery
{
    /// <summary>
    /// Unique identifier for the delivery.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Webhook ID this delivery belongs to.
    /// </summary>
    [JsonPropertyName("webhookId")]
    public string WebhookId { get; set; } = string.Empty;

    /// <summary>
    /// Event that triggered the delivery.
    /// </summary>
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Delivery status.
    /// </summary>
    [JsonPropertyName("status")]
    public DeliveryStatus Status { get; set; }

    /// <summary>
    /// HTTP status code of the response.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int? StatusCode { get; set; }

    /// <summary>
    /// Number of delivery attempts.
    /// </summary>
    [JsonPropertyName("attempts")]
    public int Attempts { get; set; }

    /// <summary>
    /// When the next retry will occur.
    /// </summary>
    [JsonPropertyName("nextRetry")]
    public DateTime? NextRetry { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// When the delivery was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Result of testing a webhook.
/// </summary>
public class WebhookTestResult
{
    /// <summary>
    /// Whether the test was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// HTTP status code received.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int? StatusCode { get; set; }

    /// <summary>
    /// Response body received.
    /// </summary>
    [JsonPropertyName("response")]
    public string? Response { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

/// <summary>
/// Information about a webhook event type.
/// </summary>
public class WebhookEventType
{
    /// <summary>
    /// Event type name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Example payload schema.
    /// </summary>
    [JsonPropertyName("examplePayload")]
    public Dictionary<string, object>? ExamplePayload { get; set; }
}

/// <summary>
/// Webhook statistics.
/// </summary>
public class WebhookStats
{
    /// <summary>
    /// Total number of deliveries.
    /// </summary>
    [JsonPropertyName("totalDeliveries")]
    public int TotalDeliveries { get; set; }

    /// <summary>
    /// Success rate (0-1).
    /// </summary>
    [JsonPropertyName("successRate")]
    public double SuccessRate { get; set; }

    /// <summary>
    /// Average delivery latency in milliseconds.
    /// </summary>
    [JsonPropertyName("avgLatency")]
    public double AvgLatency { get; set; }

    /// <summary>
    /// Breakdown by event type.
    /// </summary>
    [JsonPropertyName("byEvent")]
    public Dictionary<string, int>? ByEvent { get; set; }
}

/// <summary>
/// Request parameters for listing webhooks.
/// </summary>
public class ListWebhooksRequest
{
    /// <summary>
    /// Maximum number of results.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Page number.
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    [JsonPropertyName("active")]
    public bool? Active { get; set; }
}
