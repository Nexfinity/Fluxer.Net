using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class GuildMemberCountGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("online_count")]
    public int OnlineCount { get; set; }
}
public class GuildChannelMemberCountGatewayData : GuildMemberCountGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}