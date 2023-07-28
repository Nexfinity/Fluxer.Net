using Newtonsoft.Json;

namespace Squll.Net.Gateway;

public class ReconnectGatewayData : IGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    [JsonProperty("seq")]
    public int Sequence { get; set; }

}
