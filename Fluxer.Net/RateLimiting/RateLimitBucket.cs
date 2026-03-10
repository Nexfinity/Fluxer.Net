namespace Fluxer.Net.RateLimiting;

/// <summary>
/// Represents a sliding window rate limit bucket for API requests
/// </summary>
public class RateLimitBucket
{
    private readonly string _bucketKey;
    private readonly int _limit;
    private readonly int _windowMs;
    private readonly bool _exemptFromGlobal;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly List<long> _requestTimestamps = new();

    public string BucketKey => _bucketKey;
    public int Limit => _limit;
    public int WindowMs => _windowMs;
    public bool ExemptFromGlobal => _exemptFromGlobal;

    public RateLimitBucket(string bucketKey, int limit, int windowMs, bool exemptFromGlobal = false)
    {
        _bucketKey = bucketKey;
        _limit = limit;
        _windowMs = windowMs;
        _exemptFromGlobal = exemptFromGlobal;
    }

    /// <summary>
    /// Attempts to acquire a rate limit slot, waiting if necessary
    /// </summary>
    /// <returns>Time in milliseconds until the request can proceed (0 if immediate)</returns>
    public async Task<int> AcquireAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var windowStart = now - _windowMs;

            // Remove timestamps outside the current window
            _requestTimestamps.RemoveAll(ts => ts < windowStart);

            if (_requestTimestamps.Count >= _limit)
            {
                // Calculate how long to wait
                var oldestInWindow = _requestTimestamps.Min();
                var waitTime = (int)(oldestInWindow + _windowMs - now);
                return waitTime > 0 ? waitTime : 0;
            }

            // Add current timestamp
            _requestTimestamps.Add(now);
            return 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets the number of available requests in the current window
    /// </summary>
    public async Task<int> GetRemainingAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var windowStart = now - _windowMs;

            // Remove timestamps outside the current window
            _requestTimestamps.RemoveAll(ts => ts < windowStart);

            return Math.Max(0, _limit - _requestTimestamps.Count);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets the time in milliseconds until the next slot becomes available
    /// </summary>
    public async Task<int> GetResetTimeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_requestTimestamps.Count == 0)
                return 0;

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var oldestInWindow = _requestTimestamps.Min();
            var resetTime = oldestInWindow + _windowMs - now;

            return resetTime > 0 ? (int)resetTime : 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Clears all tracked timestamps (useful for testing or manual reset)
    /// </summary>
    public async Task ResetAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _requestTimestamps.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
