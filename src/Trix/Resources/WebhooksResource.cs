using System.Runtime.CompilerServices;
using System.Text.Json;
using Trix.Exceptions;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for webhook management.
/// </summary>
public class WebhooksResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the WebhooksResource.
    /// </summary>
    internal WebhooksResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Creates a new webhook.
    /// </summary>
    /// <param name="request">The webhook to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created webhook.</returns>
    public virtual async Task<Webhook> CreateAsync(
        CreateWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<Webhook>("/v1/webhooks", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a webhook by ID.
    /// </summary>
    /// <param name="id">The webhook ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The webhook.</returns>
    public virtual async Task<Webhook> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return await GetAsync<Webhook>($"/v1/webhooks/{Uri.EscapeDataString(id)}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a webhook.
    /// </summary>
    /// <param name="id">The webhook ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated webhook.</returns>
    public virtual async Task<Webhook> UpdateAsync(
        string id,
        UpdateWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<Webhook>($"/v1/webhooks/{Uri.EscapeDataString(id)}", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a webhook.
    /// </summary>
    /// <param name="id">The webhook ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual new async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        await base.DeleteAsync($"/v1/webhooks/{Uri.EscapeDataString(id)}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists webhooks.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of webhooks.</returns>
    public virtual async Task<PaginatedResponse<Webhook>> ListAsync(
        ListWebhooksRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("limit", request?.Limit),
            ("page", request?.Page),
            ("active", request?.Active)
        );

        return await GetAsync<PaginatedResponse<Webhook>>("/v1/webhooks", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all webhooks with automatic pagination.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of webhooks.</returns>
    public virtual async IAsyncEnumerable<Webhook> ListAllAsync(
        ListWebhooksRequest? request = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;
        var limit = request?.Limit ?? 100;
        int pages = 0;
        const int MaxPages = 1000;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

            var response = await ListAsync(new ListWebhooksRequest
            {
                Limit = limit,
                Page = page,
                Active = request?.Active
            }, cancellationToken).ConfigureAwait(false);

            foreach (var webhook in response.Data)
            {
                yield return webhook;
            }

            if (response.Pagination?.HasMore != true)
            {
                break;
            }

            page++;
        }
    }

    /// <summary>
    /// Tests a webhook by sending a test event.
    /// </summary>
    /// <param name="id">The webhook ID.</param>
    /// <param name="eventType">Optional event type to test.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The test result.</returns>
    public virtual async Task<WebhookTestResult> TestAsync(
        string id,
        string? eventType = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var request = eventType != null ? new { eventType } : null;
        return await PostAsync<WebhookTestResult>($"/v1/webhooks/{Uri.EscapeDataString(id)}/test", request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets delivery history for a webhook.
    /// </summary>
    /// <param name="id">The webhook ID.</param>
    /// <param name="limit">Maximum number of deliveries.</param>
    /// <param name="page">Page number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of deliveries.</returns>
    public virtual async Task<PaginatedResponse<WebhookDelivery>> GetDeliveriesAsync(
        string id,
        int? limit = null,
        int? page = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var queryParams = BuildQueryParams(
            ("limit", limit),
            ("page", page)
        );

        return await GetAsync<PaginatedResponse<WebhookDelivery>>($"/v1/webhooks/{Uri.EscapeDataString(id)}/deliveries", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retries a failed webhook delivery.
    /// </summary>
    /// <param name="webhookId">The webhook ID.</param>
    /// <param name="deliveryId">The delivery ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The retried delivery.</returns>
    public virtual async Task<WebhookDelivery> RetryDeliveryAsync(
        string webhookId,
        string deliveryId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(webhookId);
        ArgumentException.ThrowIfNullOrEmpty(deliveryId);

        return await PostAsync<WebhookDelivery>($"/v1/webhooks/{Uri.EscapeDataString(webhookId)}/deliveries/{Uri.EscapeDataString(deliveryId)}/retry", null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets available webhook event types.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of event types.</returns>
    public virtual async Task<List<WebhookEventType>> GetEventTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<WebhookEventType>>("/v1/webhooks/event-types", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets webhook statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Webhook statistics.</returns>
    public virtual async Task<WebhookStats> GetStatsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<WebhookStats>("/v1/webhooks/stats", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies the HMAC-SHA256 signature of an inbound webhook request.
    /// </summary>
    /// <remarks>
    /// Recomputes the signature over <c>{timestamp}.{payload}</c> using the endpoint
    /// signing secret and compares it to the <c>X-Webhook-Signature</c> header in constant
    /// time. Requests whose timestamp differs from now by more than
    /// <paramref name="toleranceSeconds"/> are rejected to prevent replay. Pass the
    /// <b>raw</b> request body exactly as received — re-serializing it changes the bytes
    /// and fails verification. Static so receivers need not construct a client.
    /// </remarks>
    /// <param name="payload">The raw webhook request body.</param>
    /// <param name="signatureHeader">The <c>X-Webhook-Signature</c> header value (<c>t=...,v1=...</c>).</param>
    /// <param name="secret">The endpoint's signing secret.</param>
    /// <param name="toleranceSeconds">Maximum accepted age of the signature, in seconds (default 300).</param>
    /// <returns><c>true</c> if the signature is valid and within tolerance; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="payload"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="secret"/> is null or empty.</exception>
    /// <example>
    /// <code>
    /// var raw = await new StreamReader(Request.Body).ReadToEndAsync();
    /// string header = Request.Headers["X-Webhook-Signature"];
    /// if (!WebhooksResource.VerifySignature(raw, header, mySigningSecret))
    ///     return Results.Unauthorized();
    /// </code>
    /// </example>
    public static bool VerifySignature(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = 300)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentException.ThrowIfNullOrEmpty(secret);
        return WebhookSignature.Verify(payload, signatureHeader, secret, toleranceSeconds);
    }

    /// <summary>
    /// Verifies an inbound webhook signature and deserializes the trusted payload to <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Combines <see cref="VerifySignature(string, string, string, int)"/> with JSON
    /// deserialization: use it when you want the typed event and want any invalid, expired,
    /// or tampered request to fail loudly rather than return a bool.
    /// </remarks>
    /// <typeparam name="T">The event payload type to deserialize into.</typeparam>
    /// <param name="payload">The raw webhook request body.</param>
    /// <param name="signatureHeader">The <c>X-Webhook-Signature</c> header value (<c>t=...,v1=...</c>).</param>
    /// <param name="secret">The endpoint's signing secret.</param>
    /// <returns>The deserialized, signature-verified payload.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="payload"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="secret"/> is null or empty.</exception>
    /// <exception cref="TrixWebhookVerificationException">
    /// The signature is missing, malformed, expired, or does not match — or the verified
    /// body cannot be deserialized to <typeparamref name="T"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// var evt = WebhooksResource.Unwrap&lt;MemoryCreatedEvent&gt;(raw, header, mySigningSecret);
    /// Console.WriteLine(evt.Data.Id);
    /// </code>
    /// </example>
    public static T Unwrap<T>(string payload, string signatureHeader, string secret)
    {
        if (!VerifySignature(payload, signatureHeader, secret))
            throw new TrixWebhookVerificationException();
        return DeserializeVerified<T>(payload);
    }

    /// <summary>
    /// Deserializes an already-verified payload, mapping JSON failures to
    /// <see cref="TrixWebhookVerificationException"/> so callers have a single failure mode.
    /// </summary>
    private static T DeserializeVerified<T>(string payload)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(payload, JsonOptions)
                ?? throw new TrixWebhookVerificationException("Webhook payload deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new TrixWebhookVerificationException("Webhook payload could not be deserialized.", ex);
        }
    }
}
