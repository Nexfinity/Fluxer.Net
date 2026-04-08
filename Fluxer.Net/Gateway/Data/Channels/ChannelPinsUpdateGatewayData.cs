using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Channels;

public class ChannelPinsUpdateGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("last_pin_timestamp")]
    public DateTime? LastPinAt { get; set; }
}
