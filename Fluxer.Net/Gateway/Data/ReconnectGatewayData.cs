using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class ReconnectGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    [JsonProperty("seq")]
    public int Sequence { get; set; }

}
