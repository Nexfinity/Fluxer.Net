# Fluxer.Net Rate Limiting System

This directory contains the client-side rate limiting implementation for the Fluxer.Net library, which uses a **sliding window algorithm** to prevent exceeding API rate limits.

## Overview

The rate limiting system automatically manages API request throttling to match Fluxer's server-side rate limits. It prevents your application from being rate-limited by the server by tracking requests locally and waiting when necessary.

## Architecture

### Components

1. **RateLimitBucket** - Represents a single rate limit bucket using the sliding window algorithm
   - Tracks request timestamps within a time window
   - Automatically removes expired timestamps
   - Thread-safe using `SemaphoreSlim`

2. **RateLimitConfig** - Configuration for a rate limit bucket
   - `Bucket`: Bucket identifier (e.g., "channel:message:create::channel_id")
   - `Limit`: Maximum requests allowed in the window
   - `WindowMs`: Time window in milliseconds
   - `ExemptFromGlobal`: Whether this bucket is exempt from global rate limits

3. **RateLimitManager** - Manages all rate limit buckets
   - Creates and caches buckets dynamically
   - Handles parameter substitution (channel_id, guild_id, etc.)
   - Provides async waiting for rate limits

4. **RateLimitConfigs** - Static class containing all bucket configurations
   - Matches the official Fluxer API rate limit configuration
   - Organized by API category (auth, channels, guilds, users, etc.)

5. **RateLimitMappings** - Maps API routes to their rate limit configurations
   - Centralized reference for developers
   - Includes usage examples

## Usage

### Basic Configuration

Rate limiting is **enabled by default**. To disable it:

```csharp
var config = new FluxerConfig
{
    EnableRateLimiting = false  // Disable client-side rate limiting
};

var apiClient = new ApiClient("your-token", config);
```

### Accessing Rate Limit Information

The `RateLimitManager` is exposed on the `ApiClient`:

```csharp
var apiClient = new ApiClient("your-token", new FluxerConfig());

// Get a bucket for a specific channel
var bucket = apiClient.RateLimitManager.GetBucket(
    RateLimitConfigs.CHANNEL_MESSAGE_CREATE,
    channelId: 123456789
);

// Check remaining requests
var (remaining, resetMs) = await apiClient.RateLimitManager.GetBucketInfoAsync(bucket);
Console.WriteLine($"Remaining requests: {remaining}, Reset in: {resetMs}ms");

// Get active bucket count
int activeBuckets = apiClient.RateLimitManager.ActiveBucketCount;
```

### Implementing Rate Limiting in Custom Methods

If you're extending the `ApiClient`, use the `WaitForRateLimit` helper:

```csharp
public async Task<Message> PostChannelMessage(ulong channelId, Message message)
{
    // Wait for rate limit before making the request
    await WaitForRateLimit(RateLimitConfigs.CHANNEL_MESSAGE_CREATE, channelId: channelId);

    return await MakeFluxerApiRequestRS<Message, Message>(
        HttpMethod.Post,
        $"/channels/{channelId}/messages",
        message,
        true
    );
}
```

## Rate Limit Buckets

### Bucket Types

Buckets are categorized by resource type:

- **Global buckets**: Apply to all requests (e.g., `auth:login`)
- **Resource-specific buckets**: Apply to specific resources (e.g., `channel:message:create::123456`)

### Dynamic Parameter Substitution

The system automatically substitutes parameters in bucket keys:

- `::channel_id` → Replaced with actual channel ID
- `::guild_id` → Replaced with actual guild ID
- `::user_id` / `::target_id` → Replaced with actual user ID
- `::webhook_id` → Replaced with actual webhook ID
- `::invite_code` → Replaced with actual invite code

Example:
```csharp
// Config: "channel:message:create::channel_id"
// With channelId=123456
// Actual bucket: "channel:message:create::123456"
```

## Rate Limit Configuration Examples

### Auth Limits
- `AUTH_LOGIN`: 10 requests / 10 seconds
- `AUTH_REGISTER`: 10 requests / 10 seconds
- `AUTH_FORGOT_PASSWORD`: 5 requests / 60 seconds

### Channel Limits
- `CHANNEL_MESSAGE_CREATE`: 20 requests / 10 seconds per channel
- `CHANNEL_MESSAGE_GET`: 100 requests / 10 seconds per channel
- `CHANNEL_TYPING`: 20 requests / 10 seconds per channel

### Guild Limits
- `GUILD_CREATE`: 10 requests / 60 seconds
- `GUILD_MEMBER_UPDATE`: 20 requests / 10 seconds per guild
- `GUILD_EMOJI_CREATE`: 20 requests / 10 seconds per guild

### User Limits
- `USER_UPDATE_SELF`: 20 requests / 60 seconds
- `USER_ACCOUNT_DELETE`: 5 requests / 1 hour
- `USER_DATA_HARVEST`: 1 request / 1 hour

## Sliding Window Algorithm

The sliding window algorithm tracks exact request timestamps:

1. Before each request, check if there are `< limit` requests in the last `windowMs` milliseconds
2. If under the limit, add the current timestamp and proceed
3. If at the limit, calculate wait time based on the oldest timestamp in the window
4. Wait for the oldest timestamp to expire from the window, then proceed

### Advantages over Fixed Window

- **No burst issues**: Prevents request bursts at window boundaries
- **More accurate**: Tracks exact request times, not just counts
- **Fairer**: Spreads requests evenly across time

## Testing

### Clear All Buckets

Useful for testing or manual reset:

```csharp
await apiClient.RateLimitManager.ClearAllBucketsAsync();
```

### Disable for Testing

```csharp
var config = new FluxerConfig
{
    EnableRateLimiting = false
};
```

## Performance Considerations

- **Memory**: Each bucket stores timestamps (8 bytes per timestamp)
- **CPU**: O(n) cleanup of expired timestamps on each request (n = limit)
- **Concurrency**: Thread-safe using `SemaphoreSlim`, minimal contention

Typical memory usage: ~1KB per active bucket with 100 requests in the window.

## Complete Bucket Reference

See `RateLimitConfigs.cs` for all 100+ configured rate limit buckets matching the official Fluxer API.

See `RateLimitMappings.cs` for route-to-bucket mappings.

## Changelog

### v0.4.0
- Initial rate limiting implementation
- Sliding window algorithm
- All Fluxer API routes configured
- Configurable enable/disable
- Thread-safe bucket management
