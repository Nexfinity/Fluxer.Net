using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class EntityRemovedGatewayData
{
    [JsonProperty("id")]
    public ulong? Id { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }
}
