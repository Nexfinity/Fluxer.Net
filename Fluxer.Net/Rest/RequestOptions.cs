namespace Fluxer.Net.Rest;

public class RequestOptions
{
    /// <summary>
    /// Add audit log reason to this request, only used for certain endpoints.
    /// </summary>
    public string AuditLogReason { get; set; }

    /// <summary>
    /// Which failures this request waits out and retries. Falls back to <see cref="FluxerConfig.DefaultRetryMode"/>.
    /// </summary>
    public RetryMode? RetryMode { get; set; }

    /// <summary>
    /// How long, in milliseconds, this request may spend waiting for rate limits before
    /// <see cref="RateLimitedException"/> is thrown. Falls back to <see cref="FluxerConfig.DefaultRequestTimeout"/>.
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// Cancels the request, including any wait for a rate limit.
    /// </summary>
    public CancellationToken CancelToken { get; set; } = CancellationToken.None;

    /// <summary>
    /// Called with the rate limit headers of every response to this request. Falls back to
    /// <see cref="FluxerConfig.DefaultRatelimitCallback"/>.
    /// </summary>
    public Func<IRateLimitInfo, Task> RatelimitCallback { get; set; }
}
