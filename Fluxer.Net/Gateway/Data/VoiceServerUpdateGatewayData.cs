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
}
