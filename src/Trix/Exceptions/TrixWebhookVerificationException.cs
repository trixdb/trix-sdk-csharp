namespace Trix.Exceptions;

/// <summary>
/// Exception thrown when an inbound webhook payload fails signature verification.
/// </summary>
/// <remarks>
/// Raised by <see cref="Trix.Resources.WebhooksResource.Unwrap{T}(string, string, string)"/>
/// when the <c>X-Webhook-Signature</c> header is missing, malformed, expired, or does not
/// match the payload (or the verified body cannot be deserialized). Treat the request as
/// untrusted and reject it (for example, respond with HTTP 400 or 401).
/// </remarks>
public class TrixWebhookVerificationException : TrixException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrixWebhookVerificationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public TrixWebhookVerificationException(
        string message = "Webhook signature verification failed.")
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrixWebhookVerificationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TrixWebhookVerificationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
