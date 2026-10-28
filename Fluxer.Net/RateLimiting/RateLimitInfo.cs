using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

/// <summary>
/// The rate limit headers of one response, and the delay from the body of a 429.
/// </summary>
public readonly struct RateLimitInfo : IRateLimitInfo
{
    /// <inheritdoc />
    public bool IsGlobal { get; }

    /// <inheritdoc />
    public int? Limit { get; }

    /// <inheritdoc />
    public int? Remaining { get; }

    /// <inheritdoc />
    public int? RetryAfter { get; }

    /// <inheritdoc />
    public DateTimeOffset? Reset { get; }

    /// <inheritdoc />
    public TimeSpan? ResetAfter { get; }

    /// <inheritdoc />
    public string Bucket { get; }

    /// <inheritdoc />
    public string Scope { get; }

    /// <inheritdoc />
    public TimeSpan? Lag { get; }

    /// <inheritdoc />
    public string Endpoint { get; }

    internal RateLimitInfo(HttpResponseMessage response, string endpoint, string body)
    {
        Endpoint = endpoint;
        IsGlobal = string.Equals(Header(response, "X-RateLimit-Global"), "true", StringComparison.OrdinalIgnoreCase);
        Limit = int.TryParse(Header(response, "X-RateLimit-Limit"), NumberStyles.None, CultureInfo.InvariantCulture, out int limit)
            ? limit
            : null;
        Remaining = int.TryParse(Header(response, "X-RateLimit-Remaining"), NumberStyles.None, CultureInfo.InvariantCulture, out int remaining)
            ? remaining
            : null;
        Reset = double.TryParse(Header(response, "X-RateLimit-Reset"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double reset)
            ? DateTimeOffset.FromUnixTimeMilliseconds((long)(reset * 1000))
            : null;
        ResetAfter = double.TryParse(Header(response, "X-RateLimit-Reset-After"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double resetAfter)
            ? TimeSpan.FromSeconds(resetAfter)
            : null;
        Bucket = Header(response, "X-RateLimit-Bucket");
        Scope = Header(response, "X-RateLimit-Scope");
        Lag = response.Headers.Date is DateTimeOffset date ? DateTimeOffset.UtcNow - date : null;

        double? retryAfter = ReadBodyRetryAfter(body);
        if (retryAfter is null && double.TryParse(Header(response, "Retry-After"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double headerRetry))
            retryAfter = headerRetry;

        RetryAfter = retryAfter is double seconds ? (int)Math.Ceiling(seconds * 1000) : null;
        if (!IsGlobal && body != null && ReadBodyGlobal(body))
            IsGlobal = true;
    }

    private static string Header(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out IEnumerable<string> values) ? values.FirstOrDefault() : null;

    private static double? ReadBodyRetryAfter(string body)
    {
        if (string.IsNullOrEmpty(body))
            return null;

        try
        {
            return JObject.Parse(body).Value<double?>("retry_after");
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool ReadBodyGlobal(string body)
    {
        try
        {
            return JObject.Parse(body).Value<bool?>("global") == true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
