namespace Fluxer.Net;

/// <summary>
/// What a response said about the rate limit its request was counted against.
/// </summary>
public interface IRateLimitInfo
{
    /// <summary>
    /// Gets whether the global limit, rather than the route's own, refused the request.
    /// </summary>
    bool IsGlobal { get; }

    /// <summary>
    /// Gets how many requests the route's bucket admits in one window.
    /// </summary>
    int? Limit { get; }

    /// <summary>
    /// Gets how many more requests the route's bucket admits right now.
    /// </summary>
    int? Remaining { get; }

    /// <summary>
    /// Gets how long to wait before one more request is admitted, in milliseconds, when the response gave a delay.
    /// </summary>
    int? RetryAfter { get; }

    /// <summary>
    /// Gets when the route's bucket has fully drained.
    /// </summary>
    DateTimeOffset? Reset { get; }

    /// <summary>
    /// Gets how long until the route's bucket has fully drained.
    /// </summary>
    TimeSpan? ResetAfter { get; }

    /// <summary>
    /// Gets Fluxer's identifier for the route's bucket.
    /// </summary>
    string Bucket { get; }

    /// <summary>
    /// Gets the X-RateLimit-Scope header, which names the allowance that refused the request, such as user, global or shared.
    /// </summary>
    string Scope { get; }

    /// <summary>
    /// Gets how far the local clock is ahead of Fluxer's, when the response carried a date.
    /// </summary>
    TimeSpan? Lag { get; }

    /// <summary>
    /// Gets the request's method and route.
    /// </summary>
    string Endpoint { get; }
}
