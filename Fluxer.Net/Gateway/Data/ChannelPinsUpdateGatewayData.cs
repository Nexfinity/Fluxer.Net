using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class ChannelPinsUpdateGatewayData : IGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("last_pin_timestamp")]
    public DateTime? LastPinTimestamp { get; set; }
}
