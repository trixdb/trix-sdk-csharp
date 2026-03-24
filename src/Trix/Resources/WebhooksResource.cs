using System.Runtime.CompilerServices;
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
}
