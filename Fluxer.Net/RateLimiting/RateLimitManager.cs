using System.Collections.Concurrent;
using Serilog;

namespace Fluxer.Net.RateLimiting;

/// <summary>
/// Manages rate limit buckets for API requests with sliding window algorithm
/// </summary>
public class RateLimitManager
{
    private readonly ConcurrentDictionary<string, RateLimitBucket> _buckets = new();
    private readonly bool _enableRateLimiting;

    public RateLimitManager(bool enableRateLimiting = true)
    {
        _enableRateLimiting = enableRateLimiting;
    }

    /// <summary>
    /// Gets or creates a rate limit bucket for the given configuration and dynamic parameters
    /// </summary>
    /// <param name="config">The rate limit configuration</param>
    /// <param name="channelId">Optional channel ID for channel-specific buckets</param>
    /// <param name="guildId">Optional guild ID for guild-specific buckets</param>
    /// <param name="userId">Optional user ID for user-specific buckets</param>
    /// <param name="webhookId">Optional webhook ID for webhook-specific buckets</param>
    /// <param name="inviteCode">Optional invite code for invite-specific buckets</param>
    /// <returns>The rate limit bucket</returns>
    public RateLimitBucket GetBucket(
        RateLimitConfig config,
        ulong? channelId = null,
        ulong? guildId = null,
        ulong? userId = null,
        ulong? webhookId = null,
        string inviteCode = null)
    {
        if (!_enableRateLimiting)
            return null;

        // Build the actual bucket key by replacing placeholders
        var bucketKey = config.Bucket;

        if (channelId.HasValue && bucketKey.Contains("::channel_id"))
            bucketKey = bucketKey.Replace("::channel_id", $"::{channelId.Value}");

        if (guildId.HasValue && bucketKey.Contains("::guild_id"))
            bucketKey = bucketKey.Replace("::guild_id", $"::{guildId.Value}");

        if (userId.HasValue && bucketKey.Contains("::user_id"))
            bucketKey = bucketKey.Replace("::user_id", $"::{userId.Value}");

        if (userId.HasValue && bucketKey.Contains("::target_id"))
            bucketKey = bucketKey.Replace("::target_id", $"::{userId.Value}");

        if (webhookId.HasValue && bucketKey.Contains("::webhook_id"))
            bucketKey = bucketKey.Replace("::webhook_id", $"::{webhookId.Value}");

        if (!string.IsNullOrEmpty(inviteCode) && bucketKey.Contains("::invite_code"))
            bucketKey = bucketKey.Replace("::invite_code", $"::{inviteCode}");

        return _buckets.GetOrAdd(bucketKey, _ => new RateLimitBucket(
            bucketKey,
            config.Limit,
            config.WindowMs,
            config.ExemptFromGlobal
        ));
    }

    /// <summary>
    /// Waits for rate limit if necessary before proceeding with request
    /// </summary>
    /// <param name="bucket">The rate limit bucket to check</param>
    /// <returns>Task that completes when request can proceed</returns>
    public async Task WaitForRateLimitAsync(RateLimitBucket bucket)
    {
        if (!_enableRateLimiting || bucket == null)
            return;

        var waitTime = await bucket.AcquireAsync();

        if (waitTime > 0)
        {
            Log.Warning("Rate limit hit for bucket {Bucket}. Waiting {WaitTime}ms before retry", bucket.BucketKey, waitTime);
            await Task.Delay(waitTime);

            // Try again after waiting
            var retryWaitTime = await bucket.AcquireAsync();
            if (retryWaitTime > 0)
            {
                Log.Warning("Rate limit still active for bucket {Bucket}. Waiting additional {WaitTime}ms", bucket.BucketKey, retryWaitTime);
                await Task.Delay(retryWaitTime);
            }
        }
    }

    /// <summary>
    /// Gets information about a specific bucket
    /// </summary>
    public async Task<(int remaining, int resetMs)> GetBucketInfoAsync(RateLimitBucket bucket)
    {
        if (!_enableRateLimiting || bucket == null)
            return (int.MaxValue, 0);

        var remaining = await bucket.GetRemainingAsync();
        var resetMs = await bucket.GetResetTimeAsync();

        return (remaining, resetMs);
    }

    /// <summary>
    /// Clears all rate limit buckets (useful for testing)
    /// </summary>
    public async Task ClearAllBucketsAsync()
    {
        foreach (RateLimitBucket bucket in _buckets.Values)
        {
            await bucket.ResetAsync();
        }

        _buckets.Clear();
    }

    /// <summary>
    /// Gets the total number of active buckets
    /// </summary>
    public int ActiveBucketCount => _buckets.Count;
}
