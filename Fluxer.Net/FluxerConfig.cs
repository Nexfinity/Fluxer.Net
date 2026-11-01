using Serilog;
using System.Net;

namespace Fluxer.Net;

/// <summary>
/// Configuration class for the Fluxer.Net library. Controls behavior for both ApiClient and GatewayClient
/// including endpoints, reconnection, logging, rate limiting, and gateway event handling.
/// </summary>
public class FluxerConfig
{
    /// <summary>
    /// Number of seconds to wait between gateway reconnection attempts. Defaults to 10 seconds.
    /// </summary>
    public int ReconnectAttemptDelay { get; set; } = 10;

    /// <summary>
    /// Custom Serilog logger configuration for the library. If null, the library will use the
    /// application's existing Serilog configuration. Allows for separate log levels and sinks
    /// for Fluxer.Net operations.
    /// </summary>
    public ILogger? RestSerilog { get; set; }

    /// <summary>
    /// Custom Serilog logger configuration for the library. If null, the library will use the
    /// application's existing Serilog configuration. Allows for separate log levels and sinks
    /// for Fluxer.Net operations.
    /// </summary>
    public ILogger? GatewaySerilog { get; set; }

    /// <summary>
    /// Base URL for the Fluxer REST API. The {v} placeholder is replaced with <see cref="Version"/>.
    /// Defaults to "https://api.fluxer.app/v{v}/".
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://api.fluxer.app/v{v}";

    /// <summary>
    /// API version number to use. Defaults to 1, which is currently the only supported version.
    /// This value replaces the {v} placeholder in <see cref="ApiBaseUrl"/>.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// WebSocket gateway URL for real-time event streaming. Must use JSON encoding.
    /// Compression is not currently supported. Defaults to "wss://gateway.fluxer.app/?v=1&amp;encoding=json".
    /// </summary>
    public string GatewayUrl { get; set; } = "wss://gateway.fluxer.app/?v=1&encoding=json";

    /// <summary>
    /// Whether the media, static, web, admin, invite and gift URLs are taken from the <c>endpoints</c> an instance
    /// sends in READY, so a self-hosted instance only needs <see cref="ApiBaseUrl"/> and <see cref="GatewayUrl"/>
    /// to be set. Defaults to true. The API and gateway URLs are never changed by READY.
    /// </summary>
    public bool DiscoverEndpoints { get; set; } = true;

    /// <summary>
    /// The media URL of the main Fluxer instance, used where no client configuration is at hand.
    /// </summary>
    public const string DefaultMediaUrl = "https://fluxerusercontent.com";

    /// <summary>
    /// Points the client at a self-hosted Fluxer instance. The API URL may be given with or without the
    /// <c>/v{v}</c> version path, and the gateway URL with or without its query string; both are completed.
    /// </summary>
    /// <param name="apiUrl">The instance's public API, such as <c>https://api.example.org</c>.</param>
    /// <param name="gatewayUrl">The instance's gateway, such as <c>wss://gateway.example.org</c>.</param>
    /// <returns>This configuration.</returns>
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
    /// <param name="endpoints">The <c>endpoints</c> object of READY.</param>
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
    /// Optional HttpClient instance to use for API requests. If null, a new HttpClient will be created.
    /// Providing your own HttpClient allows for connection pooling, custom headers, and proxy configuration.
    /// </summary>
    public HttpClient HttpClient { get; set; } = null;

    /// <summary>
    /// Proxy used for the rest/http client.
    /// </summary>
    public IWebProxy? Proxy { get; set; }

    /// <summary>
    /// Whether REST requests wait for Fluxer's rate limits and are sent again when answered with 429. Defaults to true.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// Which failures REST requests retry. POST and PATCH are not resent after a timeout or 502.
    /// </summary>
    public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;

    /// <summary>
    /// Whether a bot token calling a route Fluxer only serves to users throws <see cref="Rest.UserTokenRequiredException"/>
    /// before anything is sent. Defaults to true. Turn it off to send such calls anyway, for example against an
    /// instance with different rules.
    /// </summary>
    public bool EnforceUserOnlyRoutes { get; set; } = true;

    /// <summary>
    /// Milliseconds a REST request may wait for rate limits before throwing. Null waits as long as needed.
    /// </summary>
    public int? DefaultRequestTimeout { get; set; }

    /// <summary>
    /// Called with the rate limit headers of every REST response.
    /// </summary>
    public Func<IRateLimitInfo, Task> DefaultRatelimitCallback { get; set; }

    /// <summary>
    /// REST requests sent per second before waiting. Set to 0 to rely on 429 responses alone.
    /// </summary>
    public int GlobalRateLimit { get; set; } = 50;

    /// <summary>
    /// Zero based id of this shard. Sent in IDENTIFY together with <see cref="ShardCount"/>. Leave both unset to connect without sharding.
    /// </summary>
    public int? ShardId { get; set; }

    /// <summary>
    /// Total number of shards. Sent in IDENTIFY together with <see cref="ShardId"/>. Leave both unset to connect without sharding.
    /// </summary>
    public int? ShardCount { get; set; }

    /// <summary>
    /// The shard pair sent in IDENTIFY, or null when sharding is not configured.
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
    /// Gets the fully resolved API base URL with the version number substituted.
    /// This is the actual URL used for API requests.
    /// </summary>
    public string RealApiBaseUrl { get => ApiBaseUrl.Replace("{v}", Version.ToString()); }

    /// <summary>
    /// Get the media/user content url.
    /// </summary>
    public string MediaUrl { get; set; } = "https://fluxerusercontent.com";

    /// <summary>
    /// Get the static content url.
    /// </summary>
    public string StaticUrl { get; set; } = "https://fluxerstatic.com";

    /// <summary>
    /// Get the admin url.
    /// </summary>
    public string AdminUrl { get; set; } = "https://admin.fluxer.app";

    /// <summary>
    /// Get the invite url.
    /// </summary>
    public string InviteUrl { get; set; } = "https://fluxer.gg";

    /// <summary>
    /// Get the gift url.
    /// </summary>
    public string GiftUrl { get; set; } = "https://fluxer.gift";

    /// <summary>
    /// Get the web url.
    /// </summary>
    public string WebUrl { get; set; } = "https://web.fluxer.app";

}
