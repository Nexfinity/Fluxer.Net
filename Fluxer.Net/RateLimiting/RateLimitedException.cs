namespace Fluxer.Net;

/// <summary>
/// Thrown when a request is rate limited and its <see cref="RetryMode"/> does not allow waiting, or the wait would
/// run past the request's timeout.
/// </summary>
public class RateLimitedException : TimeoutException
{
    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="endpoint">The request's method and route.</param>
    /// <param name="retryAfter">How long the request would have had to wait.</param>
    public RateLimitedException(string endpoint, TimeSpan retryAfter)
        : base($"Rate limited on {endpoint}, retry after {retryAfter.TotalMilliseconds:F0} ms")
    {
        Endpoint = endpoint;
        RetryAfter = retryAfter;
    }

    /// <summary>
    /// Gets the request's method and route.
    /// </summary>
    public string Endpoint { get; }

    /// <summary>
    /// Gets how long the request would have had to wait.
    /// </summary>
    public TimeSpan RetryAfter { get; }
}
