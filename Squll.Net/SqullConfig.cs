using System.Net.Http;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Squll.Net.Gateway.Data;

namespace Squll.Net;

public class SqullConfig
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
    ///     Base url for squll's api. Defaults to "https://squll.com/api/v{v}/". {v} is replaced with <see cref="Version"/>
    /// </summary>
    public string SqullApiBaseUrl { get; set; } = "https://api.squll.com/v{v}/";

    /// <summary>
    ///     The version of squll's api to use. Defaults to 1. The only supported value is 1.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    ///     The gateway to connect to. Encoding must be json and compression is unsupported.
    /// </summary>
    public string SqullGatewayUrl { get; set; } = "wss://gateway.squll.com?encoding=json";

    /// <summary>
    ///     Pass your applications HttpClient here if one is generated
    /// </summary>
    public HttpClient HttpClient { get; set; } = null;
    
    /// <summary>
    ///     (optionally) block some dispathes your application does not handle -- for example PRESENCE_UPDATE
    /// </summary>
    public List<string> IgnoredGatewayEvents { get; set; } = new();
    
    /// <summary>
    ///     The initial presence to send to squll
    /// </summary>
    public PresenceUpdateGatewayData? Presence { get; set; } = null;

    public string RealApiBaseUrl { get => SqullApiBaseUrl.Replace("{v}", Version.ToString()); }
}
