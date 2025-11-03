using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class MessageBulkDeleteGatewayData : IGatewayData
{
    [JsonProperty("ids")]
    public List<ulong> Ids { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}
