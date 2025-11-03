using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class WebhooksUpdateGatewayData : IGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}
