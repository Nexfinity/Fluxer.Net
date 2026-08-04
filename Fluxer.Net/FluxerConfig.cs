using Fluxer.Net.Gateway.Data.Users;
using Serilog.Core;

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
    public Logger? RestSerilog { get; set; }

    /// <summary>
    /// Custom Serilog logger configuration for the library. If null, the library will use the
    /// application's existing Serilog configuration. Allows for separate log levels and sinks
    /// for Fluxer.Net operations.
    /// </summary>
    public Logger? GatewaySerilog { get; set; }

    /// <summary>
    /// Base URL for the Fluxer REST API. The {v} placeholder is replaced with <see cref="Version"/>.
    /// Defaults to "https://api.fluxer.app/v{v}/".
    /// </summary>
    public string FluxerApiBaseUrl { get; set; } = "https://api.fluxer.app/v{v}";

    /// <summary>
    /// API version number to use. Defaults to 1, which is currently the only supported version.
    /// This value replaces the {v} placeholder in <see cref="FluxerApiBaseUrl"/>.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// WebSocket gateway URL for real-time event streaming. Must use JSON encoding.
    /// Compression is not currently supported. Defaults to "wss://gateway.fluxer.app/?v=1&amp;encoding=json".
    /// </summary>
    public string FluxerGatewayUrl { get; set; } = "wss://gateway.fluxer.app/?v=1&encoding=json";

    /// <summary>
    /// Optional HttpClient instance to use for API requests. If null, a new HttpClient will be created.
    /// Providing your own HttpClient allows for connection pooling, custom headers, and proxy configuration.
    /// </summary>
    public HttpClient HttpClient { get; set; } = null;

    /// <summary>
    /// List of gateway event dispatch types to ignore. Useful for filtering out high-volume events
    /// your application doesn't need (e.g., "PRESENCE_UPDATE", "TYPING_START"). Defaults to empty list.
    /// </summary>
    public List<string> IgnoredGatewayEvents { get; set; } = new();

    /// <summary>
    /// Initial presence data to send when connecting to the gateway. If null, no presence is sent.
    /// Allows setting your bot's or user's online status, activity, and other presence information.
    /// </summary>
    public PresenceUpdateGatewayData? Presence { get; set; } = null;

    /// <summary>
    /// Enable client-side rate limiting using a sliding window algorithm. Defaults to true.
    /// When enabled, API requests automatically wait when approaching rate limits to prevent
    /// 429 responses from the server. See RateLimiting/README.md for details.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// Gets the fully resolved API base URL with the version number substituted.
    /// This is the actual URL used for API requests.
    /// </summary>
    public string RealApiBaseUrl { get => FluxerApiBaseUrl.Replace("{v}", Version.ToString()); }

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

}
