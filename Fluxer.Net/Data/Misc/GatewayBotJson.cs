using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Gateway connection information and the recommended shard count.
/// </summary>
public class GatewayBotJson
{
    /// <summary>
    /// WebSocket URL to connect to the gateway.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; } = "";

    /// <summary>
    /// Recommended number of shards to use when connecting.
    /// </summary>
    [JsonProperty("shards")]
    public int Shards { get; set; }

    /// <summary>
    /// Session start rate limit information.
    /// </summary>
    [JsonProperty("session_start_limit")]
    public GatewaySessionStartLimitJson? SessionStartLimit { get; set; }
}

/// <summary>
/// Session start rate limit information.
/// </summary>
public class GatewaySessionStartLimitJson
{
    /// <summary>
    /// Total number of session starts allowed.
    /// </summary>
    [JsonProperty("total")]
    public int Total { get; set; }

    /// <summary>
    /// Remaining number of session starts.
    /// </summary>
    [JsonProperty("remaining")]
    public int Remaining { get; set; }

    /// <summary>
    /// Milliseconds until the limit resets.
    /// </summary>
    [JsonProperty("reset_after")]
    public int ResetAfter { get; set; }

    /// <summary>
    /// Maximum number of concurrent IDENTIFY requests.
    /// </summary>
    [JsonProperty("max_concurrency")]
    public int MaxConcurrency { get; set; }
}
