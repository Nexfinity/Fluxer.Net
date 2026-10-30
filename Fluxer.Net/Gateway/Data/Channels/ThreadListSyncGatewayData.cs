using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ThreadListSyncGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("threads")]
    public ChannelGatewayData[]? Threads { get; set; }

    [JsonProperty("members")]
    public ThreadMemberJson[]? Members { get; set; }

    [JsonProperty("channel_ids")]
    public ulong[]? ChannelIds { get; set; }
}
