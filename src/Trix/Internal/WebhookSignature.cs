using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Trix.Internal;

/// <summary>
/// Verifies inbound webhook HMAC-SHA256 signatures produced by the Trix API.
/// Mirrors the server scheme: <c>t=&lt;unix_seconds&gt;,v1=&lt;hex_hmac&gt;</c> where the
/// HMAC is computed over <c>{timestamp}.{payload}</c> keyed by the endpoint secret.
/// </summary>
internal static partial class WebhookSignature
{
    [GeneratedRegex(@"^t=([0-9]+),v1=([a-f0-9]+)$")]
    private static partial Regex HeaderPattern();

    /// <summary>
    /// Recomputes the signature and compares it in constant time, enforcing the replay
    /// tolerance. Callers guarantee <paramref name="payload"/> and <paramref name="secret"/>
    /// are non-null; an untrusted/malformed <paramref name="signatureHeader"/> yields false.
    /// </summary>
    /// <param name="payload">The raw webhook request body.</param>
    /// <param name="signatureHeader">The <c>X-Webhook-Signature</c> header value.</param>
    /// <param name="secret">The endpoint signing secret.</param>
    /// <param name="toleranceSeconds">Maximum accepted clock skew, in seconds.</param>
    /// <returns><c>true</c> if the signature is valid and fresh; otherwise <c>false</c>.</returns>
    internal static bool Verify(string payload, string signatureHeader, string secret, int toleranceSeconds)
    {
        if (string.IsNullOrEmpty(signatureHeader))
            return false;

        var match = HeaderPattern().Match(signatureHeader);
        if (!match.Success || !long.TryParse(
                match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var timestamp))
            return false;

        if (!WithinTolerance(timestamp, toleranceSeconds))
            return false;

        return HexEqualsFixedTime(match.Groups[2].Value, ComputeHex(secret, timestamp, payload));
    }

    /// <summary>Checks the signed timestamp is within tolerance of the current time.</summary>
    private static bool WithinTolerance(long timestamp, int toleranceSeconds)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return Math.Abs(now - timestamp) <= toleranceSeconds;
    }

    /// <summary>Computes the lowercase hex HMAC-SHA256 of <c>{timestamp}.{payload}</c>.</summary>
    private static string ComputeHex(string secret, long timestamp, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{payload}"));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>Constant-time comparison of two hex signature strings.</summary>
    private static bool HexEqualsFixedTime(string provided, string expected)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(provided),
            Encoding.UTF8.GetBytes(expected));
    }
}
