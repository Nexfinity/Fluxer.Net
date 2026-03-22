using Newtonsoft.Json;

namespace Fluxer.Net;

public class GatewaySessionJson
{
    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("mobile")]
    public bool Mobile { get; set; }

    [JsonProperty("afk")]
    public bool Afk { get; set; }
}
