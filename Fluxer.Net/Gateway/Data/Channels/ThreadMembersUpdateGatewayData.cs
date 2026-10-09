using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class ThreadMembersUpdateGatewayData
{
    [JsonProperty("id")]
    public ulong ThreadId { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("added_members")]
    public ThreadMemberJson[]? AddedMembers { get; set; }

    [JsonProperty("removed_member_ids")]
    public ulong[]? RemovedMemberIds { get; set; }
}
