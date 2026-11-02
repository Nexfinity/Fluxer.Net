using Serilog;
using System.Net;

namespace Fluxer.Net;

/// <summary>
/// The settings for a Fluxer client: endpoints, logging, rate limiting, retries and sharding.
/// </summary>
public class FluxerConfig
{
    /// <summary>
    /// The minimum number of seconds between gateway reconnection attempts. Defaults to 10.
    /// </summary>
    public int ReconnectAttemptDelay { get; set; } = 10;

    /// <summary>
    /// The logger for REST requests. When null, <see cref="FluxerClient"/>, <see cref="FluxerWebhookClient"/> and
    /// <see cref="OAuth.FluxerOAuthClient"/> create a console logger at verbose level.
    /// </summary>
    public ILogger? RestSerilog { get; set; }

    /// <summary>
    /// The logger for gateway events. When null, <see cref="FluxerClient"/> creates a console logger at verbose level.
    /// </summary>
    public ILogger? GatewaySerilog { get; set; }

    /// <summary>
    /// The base URL of the Fluxer REST API. The {v} placeholder is replaced with <see cref="Version"/>.
    /// Defaults to "https://api.fluxer.app/v{v}".
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://api.fluxer.app/v{v}";

    /// <summary>
    /// The API version number. Defaults to 1. It replaces the {v} placeholder in <see cref="ApiBaseUrl"/>.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// The gateway WebSocket URL. It must use JSON encoding. Defaults to "wss://gateway.fluxer.app/?v=1&amp;encoding=json".
    /// </summary>
    public string GatewayUrl { get; set; } = "wss://gateway.fluxer.app/?v=1&encoding=json";

    /// <summary>
    /// Whether the media, static, web, admin, invite and gift URLs are taken from the <c>endpoints</c> an instance
    /// sends in READY, so a self-hosted instance only needs <see cref="ApiBaseUrl"/> and <see cref="GatewayUrl"/>
    /// to be set. Defaults to true. The API and gateway URLs are never changed by READY.
    /// </summary>
    public bool DiscoverEndpoints { get; set; } = true;

    /// <summary>
    /// The media URL of the main Fluxer instance, used by types that have no client to read <see cref="MediaUrl"/> from.
    /// </summary>
    public const string DefaultMediaUrl = "https://fluxerusercontent.com";

    /// <summary>
    /// Points the client at a Fluxer instance. The API URL may be given with or without the
    /// <c>/v{v}</c> version path, and the gateway URL with or without its query string. Both are completed.
    /// </summary>
    /// <param name="apiUrl">The instance's public API, such as <c>https://api.example.org</c>.</param>
    /// <param name="gatewayUrl">The instance's gateway, such as <c>wss://gateway.example.org</c>.</param>
    /// <returns>This configuration.</returns>
    /// <exception cref="ArgumentException">The API URL or the gateway URL is null, empty or whitespace.</exception>
    public FluxerConfig UseInstance(string apiUrl, string gatewayUrl)
    {
        if (string.IsNullOrWhiteSpace(apiUrl))
            throw new ArgumentException("The API URL is required.", nameof(apiUrl));
        if (string.IsNullOrWhiteSpace(gatewayUrl))
            throw new ArgumentException("The gateway URL is required.", nameof(gatewayUrl));

        string api = apiUrl.Trim().TrimEnd('/');
        if (!api.Contains("{v}", StringComparison.Ordinal))
            api = System.Text.RegularExpressions.Regex.IsMatch(api, @"/v\d+$") ? System.Text.RegularExpressions.Regex.Replace(api, @"/v\d+$", "/v{v}") : api + "/v{v}";
        ApiBaseUrl = api;

        string gateway = gatewayUrl.Trim();
        if (!gateway.Contains('?'))
            gateway = gateway.TrimEnd('/') + "/?v=1&encoding=json";
        GatewayUrl = gateway;
        return this;
    }

    /// <summary>
    /// Applies the endpoints an instance advertises in READY to the media, static, web, admin, invite and gift URLs.
    /// Unknown keys are ignored and the API and gateway URLs are left alone.
    /// </summary>
    /// <param name="endpoints">The <c>endpoints</c> object of READY, keyed by endpoint name.</param>
    public void ApplyEndpoints(IReadOnlyDictionary<string, string> endpoints)
    {
        static string? Clean(string? url) => string.IsNullOrWhiteSpace(url) ? null : url.Trim().TrimEnd('/');
        if (Clean(endpoints.GetValueOrDefault("media")) is { } media) MediaUrl = media;
        if (Clean(endpoints.GetValueOrDefault("static_cdn")) is { } cdn) StaticUrl = cdn;
        if (Clean(endpoints.GetValueOrDefault("admin")) is { } admin) AdminUrl = admin;
        if (Clean(endpoints.GetValueOrDefault("invite")) is { } invite) InviteUrl = invite;
        if (Clean(endpoints.GetValueOrDefault("gift")) is { } gift) GiftUrl = gift;
        if (Clean(endpoints.GetValueOrDefault("webapp")) is { } web) WebUrl = web;
    }

    /// <summary>
    /// The HttpClient used for REST requests. When null, the client creates one that uses <see cref="Proxy"/>.
    /// </summary>
    public HttpClient HttpClient { get; set; } = null;

    /// <summary>
    /// The proxy for REST requests. It is applied only when <see cref="HttpClient"/> is null.
    /// </summary>
    public IWebProxy? Proxy { get; set; }

    /// <summary>
    /// Whether REST requests go through the rate limit queue, which waits for Fluxer's rate limits and resends
    /// requests answered with 429. When false, requests are sent directly. Defaults to true.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// The failures REST requests retry by default. Only idempotent methods are resent after a timeout or 502.
    /// </summary>
    public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;

    /// <summary>
    /// Whether a bot token calling a route that Fluxer serves only to users throws <see cref="Rest.UserTokenRequiredException"/>
    /// before anything is sent. Defaults to true. When false, such calls are sent anyway.
    /// </summary>
    public bool EnforceUserOnlyRoutes { get; set; } = true;

    /// <summary>
    /// The milliseconds a REST request may wait for rate limits before it throws. Null waits as long as needed.
    /// </summary>
    public int? DefaultRequestTimeout { get; set; }

    /// <summary>
    /// Called with the rate limit information of each REST response.
    /// </summary>
    public Func<IRateLimitInfo, Task> DefaultRatelimitCallback { get; set; }

    /// <summary>
    /// The number of REST requests sent per second before requests wait. Set to 0 to rely on 429 responses alone.
    /// </summary>
    public int GlobalRateLimit { get; set; } = 50;

    /// <summary>
    /// The zero based id of this shard. Sent in IDENTIFY together with <see cref="ShardCount"/>. Leave both unset to connect without sharding.
    /// </summary>
    public int? ShardId { get; set; }

    /// <summary>
    /// The total number of shards. Sent in IDENTIFY together with <see cref="ShardId"/>. Leave both unset to connect without sharding.
    /// </summary>
    public int? ShardCount { get; set; }

    /// <summary>
    /// Gets the shard pair sent in IDENTIFY, or null when sharding is not configured.
    /// </summary>
    /// <exception cref="InvalidOperationException">Only one of <see cref="ShardId"/> and <see cref="ShardCount"/> is set, or the pair is outside 0 &lt;= id &lt; count &lt;= 16384.</exception>
    public int[]? GetShard()
    {
        if (ShardId == null && ShardCount == null)
            return null;

        if (ShardId == null || ShardCount == null)
            throw new InvalidOperationException("ShardId and ShardCount must be set together.");

        if (ShardId < 0 || ShardCount < 1 || ShardCount > 16384 || ShardId >= ShardCount)
            throw new InvalidOperationException($"Invalid shard {ShardId} of {ShardCount}.");

        return new[] { ShardId.Value, ShardCount.Value };
    }

    /// <summary>
    /// The API base URL with the version number substituted. REST requests are sent to this URL.
    /// </summary>
    public string RealApiBaseUrl { get => ApiBaseUrl.Replace("{v}", Version.ToString()); }

    /// <summary>
    /// The base URL for user media content.
    /// </summary>
    public string MediaUrl { get; set; } = "https://fluxerusercontent.com";

    /// <summary>
    /// The base URL for static content.
    /// </summary>
    public string StaticUrl { get; set; } = "https://fluxerstatic.com";

    /// <summary>
    /// The URL of the instance's admin site.
    /// </summary>
    public string AdminUrl { get; set; } = "https://admin.fluxer.app";

    /// <summary>
    /// The base URL for invite links.
    /// </summary>
    public string InviteUrl { get; set; } = "https://fluxer.gg";

    /// <summary>
    /// The base URL for gift links.
    /// </summary>
    public string GiftUrl { get; set; } = "https://fluxer.gift";

    /// <summary>
    /// The URL of the instance's web app.
    /// </summary>
    public string WebUrl { get; set; } = "https://web.fluxer.app";

}
