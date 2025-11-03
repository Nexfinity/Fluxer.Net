using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class TypingGatewayData : IGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("timestamp")]
    public ulong Timestamp { get; set; }
}
