using System.Collections.Concurrent;
using System.Net;
using Fluxer.Net.Rest;
using Serilog;

namespace Fluxer.Net.RateLimiting;

internal sealed class RequestQueue
{
    private const int MaxTransientRetries = 3;
    private static readonly TimeSpan TransientRetryDelay = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan IdleBucketLifetime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan SweepInterval = TimeSpan.FromMinutes(1);

    private readonly ConcurrentDictionary<BucketId, RateLimitBucket> _buckets = new();
    private readonly ConcurrentDictionary<string, string> _hashes = new(StringComparer.Ordinal);
    private readonly RateLimitBucket _global;
    private readonly FluxerConfig _config;
    private readonly ILogger _logger;
    private DateTimeOffset _lastSweep = DateTimeOffset.UtcNow;

    public RequestQueue(FluxerConfig config, ILogger logger)
    {
        _config = config;
        _logger = logger;
        _global = config.GlobalRateLimit > 0
            ? RateLimitBucket.Fixed(BucketId.Create("GLOBAL", "/"), config.GlobalRateLimit, TimeSpan.FromSeconds(1))
            : null;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, string route,
        RequestOptions? options = null)
    {
        CancellationToken cancelToken = options?.CancelToken ?? CancellationToken.None;
        if (!_config.EnableRateLimiting)
            return await client.SendAsync(request, cancelToken).ConfigureAwait(false);

        RetryMode mode = options?.RetryMode ?? _config.DefaultRetryMode;
        int? timeout = options?.Timeout ?? _config.DefaultRequestTimeout;
        DateTimeOffset? deadline = timeout is int ms ? DateTimeOffset.UtcNow.AddMilliseconds(ms) : null;
        Func<IRateLimitInfo, Task> callback = options?.RatelimitCallback ?? _config.DefaultRatelimitCallback;
        BucketId routeId = BucketId.Create(request.Method.Method, route);
        string endpoint = $"{routeId.HttpMethod} {LogRedaction.Redact(route)}";

        if (request.Content != null)
            await request.Content.LoadIntoBufferAsync().ConfigureAwait(false);

        SweepIdleBuckets();
        HttpRequestMessage current = request;
        int transientRetries = 0;
        while (true)
        {
            RateLimitBucket bucket = GetBucket(routeId);
            await WaitAsync(bucket, mode, deadline, endpoint, cancelToken).ConfigureAwait(false);
            if (_global != null && !routeId.IsGlobalExempt)
            {
                try
                {
                    await WaitAsync(_global, mode, deadline, endpoint, cancelToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    bucket.Abandon();
                    throw;
                }
            }

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(current, cancelToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException) when (!cancelToken.IsCancellationRequested)
            {
                bucket.Abandon();
                if ((mode & RetryMode.RetryTimeouts) == 0 || !IsIdempotent(request.Method) || transientRetries >= MaxTransientRetries)
                    throw;

                transientRetries++;
                _logger.Warning("Request to {Endpoint} timed out, sending it again", endpoint);
                await Task.Delay(TransientRetryDelay, cancelToken).ConfigureAwait(false);
                current = Clone(request);
                continue;
            }
            catch (Exception)
            {
                bucket.Abandon();
                throw;
            }

            bool limited = response.StatusCode == (HttpStatusCode)429;
            string body = limited ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : null;
            RateLimitInfo info = new(response, endpoint, body);
            Record(routeId, bucket, info, limited);

            if (callback != null)
                await callback(info).ConfigureAwait(false);

            if (limited)
            {
                TimeSpan retryAfter = TimeSpan.FromMilliseconds(info.RetryAfter ?? 1000);
                _logger.Warning("Rate limited on {Endpoint} ({Scope}), retry after {RetryAfter} ms", endpoint,
                    info.IsGlobal ? "global" : info.Scope ?? "route", (int)retryAfter.TotalMilliseconds);

                if ((mode & RetryMode.RetryRatelimit) == 0 || (deadline.HasValue && DateTimeOffset.UtcNow + retryAfter > deadline))
                {
                    response.Dispose();
                    throw new RateLimitedException(endpoint, retryAfter);
                }

                response.Dispose();
                current = Clone(request);
                continue;
            }

            if (response.StatusCode == HttpStatusCode.BadGateway && (mode & RetryMode.Retry502) != 0 &&
                IsIdempotent(request.Method) && transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                response.Dispose();
                await Task.Delay(TransientRetryDelay, cancelToken).ConfigureAwait(false);
                current = Clone(request);
                continue;
            }

            return response;
        }
    }

    private RateLimitBucket GetBucket(BucketId routeId)
    {
        BucketId id = _hashes.TryGetValue(routeId.RouteKey, out string hash) ? routeId.WithHash(hash) : routeId;
        return _buckets.GetOrAdd(id, x => new RateLimitBucket(x));
    }

    private void Record(BucketId routeId, RateLimitBucket used, RateLimitInfo info, bool limited)
    {
        if (limited && info.IsGlobal)
        {
            _global?.Update(info, true);
            used.Abandon();
            return;
        }

        if (info.Bucket != null && !used.Id.IsHashBucket)
        {
            _hashes[routeId.RouteKey] = info.Bucket;
            RateLimitBucket named = _buckets.GetOrAdd(routeId.WithHash(info.Bucket), x => new RateLimitBucket(x));
            named.Update(info, limited);
        }

        used.Update(info, limited);
    }

    private async Task WaitAsync(RateLimitBucket bucket, RetryMode mode, DateTimeOffset? deadline, string endpoint,
        CancellationToken cancelToken)
    {
        bool logged = false;
        while (!bucket.TryTake(out TimeSpan wait, out Task signal))
        {
            if ((mode & RetryMode.RetryRatelimit) == 0 || (deadline.HasValue && DateTimeOffset.UtcNow + wait > deadline))
                throw new RateLimitedException(endpoint, wait);

            if (!logged && signal is null)
            {
                _logger.Debug("Preemptive rate limit on {Bucket} for {Endpoint}, waiting {Wait} ms", bucket.Id,
                    endpoint, (int)wait.TotalMilliseconds);
                logged = true;
            }

            Task delay = Task.Delay(wait, cancelToken);
            await (signal is null ? delay : Task.WhenAny(signal, delay)).ConfigureAwait(false);
            cancelToken.ThrowIfCancellationRequested();
        }
    }

    private void SweepIdleBuckets()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (now - _lastSweep < SweepInterval)
            return;

        _lastSweep = now;
        foreach (KeyValuePair<BucketId, RateLimitBucket> entry in _buckets)
        {
            if (now - entry.Value.LastUsedAt > IdleBucketLifetime && !entry.Value.IsBusy)
                _buckets.TryRemove(entry.Key, out _);
        }
    }

    private static bool IsIdempotent(HttpMethod method) =>
        method == HttpMethod.Get || method == HttpMethod.Head || method == HttpMethod.Put ||
        method == HttpMethod.Delete || method == HttpMethod.Options;

    private static HttpRequestMessage Clone(HttpRequestMessage request)
    {
        HttpRequestMessage clone = new(request.Method, request.RequestUri)
        {
            Content = request.Content,
            Version = request.Version
        };
        foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}
