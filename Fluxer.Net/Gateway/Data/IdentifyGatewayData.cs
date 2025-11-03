using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class IdentifyGatewayData : IGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("ignored_events")]
    public List<string> IgnoredGatewayEvents { get; set; } = new();

    [JsonProperty("presence")]
    public PresenceUpdateGatewayData? Presence { get; set; }

    public IdentifyGatewayData(string token)
        => Token = token;
}
