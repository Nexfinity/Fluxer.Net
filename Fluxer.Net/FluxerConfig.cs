using System.Net.Http;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Fluxer.Net.Gateway.Data;

namespace Fluxer.Net;

public class FluxerConfig
{
    /// <summary>
    ///     how many seconds to wait between reconnect attempts
    /// </summary>
    public int ReconnectAttemptDelay { get; set; } = 10;

    /// <summary>
    ///     The configuration to use for the libraries logger. Leave null to user the developers configuration
    /// </summary>
    public Logger? Serilog { get; set; }

    /// <summary>
    ///     Base url for fluxer's api. Defaults to "https://web.fluxer.app/client-api/v{v}/". {v} is replaced with <see cref="Version"/>
    /// </summary>
    public string FluxerApiBaseUrl { get; set; } = "https://api.fluxer.app/v{v}/";

    /// <summary>
    ///     The version of fluxer's api to use. Defaults to 1. The only supported value is 1.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    ///     The gateway to connect to. Encoding must be json and compression is unsupported.
    /// </summary>
    public string FluxerGatewayUrl { get; set; } = "wss://gateway.fluxer.app/?v=1&encoding=json";

    /// <summary>
    ///     Pass your applications HttpClient here if one is generated
    /// </summary>
    public HttpClient HttpClient { get; set; } = null;

    /// <summary>
    ///     (optionally) block some dispathes your application does not handle -- for example PRESENCE_UPDATE
    /// </summary>
    public List<string> IgnoredGatewayEvents { get; set; } = new();

    /// <summary>
    ///     The initial presence to send to fluxer
    /// </summary>
    public PresenceUpdateGatewayData? Presence { get; set; } = null;

    /// <summary>
    ///     Enable client-side rate limiting using sliding window algorithm. Defaults to true.
    ///     When enabled, API requests will automatically wait when rate limits are hit.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    public string RealApiBaseUrl { get => FluxerApiBaseUrl.Replace("{v}", Version.ToString()); }
}
