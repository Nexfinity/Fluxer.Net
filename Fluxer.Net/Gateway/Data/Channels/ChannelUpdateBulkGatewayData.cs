using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ChannelUpdateBulkGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channels")]
    public List<ChannelJson> Channels { get; set; } = new();
}