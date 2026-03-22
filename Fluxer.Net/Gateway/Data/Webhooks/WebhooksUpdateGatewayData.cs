using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Webhooks;

public class WebhooksUpdateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}
