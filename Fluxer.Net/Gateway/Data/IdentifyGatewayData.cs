using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class IdentifyGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("properties")]
    public Dictionary<string, string> Properties { get; set; }

    [JsonProperty("ignored_events")]
    public List<string> IgnoredGatewayEvents { get; set; } = new();

    [JsonProperty("presence")]
    public PresenceUpdateGatewayData? Presence { get; set; }

    [JsonProperty("shard", NullValueHandling = NullValueHandling.Ignore)]
    public int[]? Shard { get; set; }

    [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
    public int? Flags { get; set; }

    public IdentifyGatewayData(string token)
        => Token = token;
}