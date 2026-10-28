namespace Fluxer.Net;

/// <summary>
/// Which failures a REST request waits out and retries instead of returning straight away.
/// </summary>
[Flags]
public enum RetryMode
{
    /// <summary>
    /// Every failure is returned as it happens. A request that would have to wait for a rate limit throws
    /// <see cref="RateLimitedException"/> instead.
    /// </summary>
    AlwaysFail = 0,

    /// <summary>
    /// Requests that time out are sent again. Only GET, HEAD, PUT, DELETE and OPTIONS are resent.
    /// </summary>
    RetryTimeouts = 1,

    /// <summary>
    /// Requests are held until their rate limit allows them, and requests answered with 429 are sent again.
    /// </summary>
    RetryRatelimit = 4,

    /// <summary>
    /// Requests answered with 502 Bad Gateway are sent again. Only GET, HEAD, PUT, DELETE and OPTIONS are resent.
    /// </summary>
    Retry502 = 8,

    /// <summary>
    /// Every retryable failure is retried.
    /// </summary>
    AlwaysRetry = RetryTimeouts | RetryRatelimit | Retry502
}
