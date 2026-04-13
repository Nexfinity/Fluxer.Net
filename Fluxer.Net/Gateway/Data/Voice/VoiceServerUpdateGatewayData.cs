using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Voice;

public class VoiceServerUpdateGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("endpoint")]
    public string Endpoint { get; set; }

    [JsonProperty("connection_id")]
    public string ConnectionId { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}
