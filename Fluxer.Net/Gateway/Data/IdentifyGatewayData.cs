using Fluxer.Net.Gateway.Data.Users;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class IdentifyGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("properties")]
    public Dictionary<string, string> Properties { get; set; }

    [JsonProperty("ignored_events")]
    public List<string> IgnoredGatewayEvents { get; set; } = new();

    [JsonProperty("presence")]
    public PresenceUpdateGatewayData? Presence { get; set; }

    public IdentifyGatewayData(string token)
        => Token = token;
}
