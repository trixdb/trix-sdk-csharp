using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using FluentAssertions;
using Trix.Exceptions;
using Trix.Resources;

namespace Trix.Tests;

/// <summary>
/// Tests for inbound webhook signature verification. The <see cref="Sign"/> helper is an
/// independent reimplementation of the server scheme (trix-api webhook-service.js) so the
/// public API is validated against a second implementation, not its own internals.
/// </summary>
public class WebhookSignatureTests
{
    private const string Secret = "whsec_test_secret";
    private const string Payload = """{"event":"memory.created","data":{"id":"mem_1"}}""";

    private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    // Mirrors webhook-service.js generateSignature: HMAC-SHA256 over "{timestamp}.{payload}".
    private static string Sign(string secret, string payload, long timestamp)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{payload}"));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"t={timestamp},v1={hex}";
    }

    [Fact]
    public void VerifySignature_CorrectlySigned_ReturnsTrue()
    {
        var header = Sign(Secret, Payload, Now());
        WebhooksResource.VerifySignature(Payload, header, Secret).Should().BeTrue();
    }

    [Fact]
    public void VerifySignature_TamperedPayload_ReturnsFalse()
    {
        var header = Sign(Secret, Payload, Now());
        // Same signature, one byte changed in the body.
        WebhooksResource.VerifySignature(Payload + " ", header, Secret).Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_WrongSecret_ReturnsFalse()
    {
        var header = Sign(Secret, Payload, Now());
        WebhooksResource.VerifySignature(Payload, header, "whsec_wrong_secret").Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_ExpiredTimestamp_ReturnsFalse()
    {
        var header = Sign(Secret, Payload, Now() - 3600);
        WebhooksResource.VerifySignature(Payload, header, Secret).Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_FutureTimestamp_ReturnsFalse()
    {
        // Replay guard is symmetric (matches JS Math.abs): far-future timestamps are rejected.
        var header = Sign(Secret, Payload, Now() + 3600);
        WebhooksResource.VerifySignature(Payload, header, Secret).Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_OldTimestampWithinLargerTolerance_ReturnsTrue()
    {
        var header = Sign(Secret, Payload, Now() - 3600);
        WebhooksResource.VerifySignature(Payload, header, Secret, toleranceSeconds: 7200)
            .Should().BeTrue();
    }

    [Theory]
    [InlineData("")]                              // missing header
    [InlineData("garbage")]                       // no structure
    [InlineData("t=,v1=abcdef")]                  // empty timestamp
    [InlineData("t=abc,v1=abcdef")]               // non-numeric timestamp
    [InlineData("v1=abcdef")]                     // missing t=
    [InlineData("t=1700000000")]                  // missing v1=
    [InlineData("t=1700000000,v1=NOTHEX")]        // non-hex signature
    [InlineData("t=1700000000;v1=abcdef")]        // wrong separator
    public void VerifySignature_MalformedHeader_ReturnsFalse(string header)
    {
        WebhooksResource.VerifySignature(Payload, header, Secret).Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_NullPayload_Throws()
    {
        var act = () => WebhooksResource.VerifySignature(null!, "t=1,v1=ab", Secret);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerifySignature_EmptySecret_Throws()
    {
        var act = () => WebhooksResource.VerifySignature(Payload, "t=1,v1=ab", "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Unwrap_ValidSignature_ReturnsDeserializedEvent()
    {
        var header = Sign(Secret, Payload, Now());
        var evt = WebhooksResource.Unwrap<TestEvent>(Payload, header, Secret);
        evt.Event.Should().Be("memory.created");
        evt.Data!.Id.Should().Be("mem_1");
    }

    [Fact]
    public void Unwrap_InvalidSignature_ThrowsTrixWebhookVerificationException()
    {
        var header = Sign(Secret, Payload, Now());
        var act = () => WebhooksResource.Unwrap<TestEvent>(Payload, header, "whsec_wrong_secret");
        act.Should().Throw<TrixWebhookVerificationException>();
    }

    [Fact]
    public void Unwrap_ExpiredSignature_ThrowsTrixWebhookVerificationException()
    {
        var header = Sign(Secret, Payload, Now() - 3600);
        var act = () => WebhooksResource.Unwrap<TestEvent>(Payload, header, Secret);
        act.Should().Throw<TrixWebhookVerificationException>();
    }

    [Fact]
    public void TrixWebhookVerificationException_IsTrixException()
    {
        new TrixWebhookVerificationException().Should().BeAssignableTo<TrixException>();
    }

    private sealed class TestEvent
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }

        [JsonPropertyName("data")]
        public TestData? Data { get; set; }
    }

    private sealed class TestData
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
