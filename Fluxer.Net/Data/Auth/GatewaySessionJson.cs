using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GatewaySessionJson : IGatewaySession
{
    /// <inheritdoc />
    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    /// <inheritdoc />
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <inheritdoc />
    [JsonProperty("mobile")]
    public bool IsMobile { get; set; }

    /// <inheritdoc />
    [JsonProperty("afk")]
    public bool IsAfk { get; set; }
}
