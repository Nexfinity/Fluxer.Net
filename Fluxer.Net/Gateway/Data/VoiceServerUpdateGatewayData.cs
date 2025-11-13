using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class VoiceServerUpdateGatewayData : IGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("endpoint")]
    public string Endpoint { get; set; }

    [JsonProperty("connection_id")]
    public string? ConnectionId { get; set; }
}
