using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Voice;

public class VoiceServerUpdateGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("endpoint")]
    public string Endpoint { get; set; }
}
