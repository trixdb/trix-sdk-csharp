using FluentAssertions;
using Trix;

namespace Trix.Tests;

/// <summary>
/// Regression tests for issue #3 — the SDK must not dispose a caller-supplied
/// HttpMessageHandler (which may be shared across clients).
/// </summary>
public class DisposalHandlerTests
{
    private sealed class TrackingHandler : HttpMessageHandler
    {
        public bool Disposed { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage());

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposed = true;
            }
            base.Dispose(disposing);
        }
    }

    [Fact]
    public void Dispose_DoesNotDisposeCallerSuppliedHandler()
    {
        var handler = new TrackingHandler();
        var options = new TrixClientOptions
        {
            ApiKey = "test_key",
            BaseUrl = "https://api.test.com",
            HttpHandler = handler,
        };
        var client = new TrixClient(options);

        client.Dispose();

        handler.Disposed.Should().BeFalse(
            "a caller-supplied handler may be shared and must outlive the client");

        // The caller still owns and can dispose it.
        handler.Dispose();
        handler.Disposed.Should().BeTrue();
    }
}
