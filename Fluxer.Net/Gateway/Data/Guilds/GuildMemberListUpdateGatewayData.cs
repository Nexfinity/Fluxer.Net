using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A member list the session subscribed to with <see cref="GuildSubscription.MemberListChannels"/> changed.
/// </summary>
public class GuildMemberListUpdateGatewayData
{
    /// <summary>
    /// The guild the list belongs to.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The list identifier, the same as the channel ID.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The channel the list is scoped to.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// Total members in the list.
    /// </summary>
    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    /// <summary>
    /// Online members in the list.
    /// </summary>
    [JsonProperty("online_count")]
    public int OnlineCount { get; set; }

    /// <summary>
    /// The group headers in list order: hoisted roles, then online, then offline.
    /// </summary>
    [JsonProperty("groups")]
    public MemberListGroup[] Groups { get; set; } = Array.Empty<MemberListGroup>();

    /// <summary>
    /// The ranges of the list to replace.
    /// </summary>
    [JsonProperty("ops")]
    public MemberListOperation[] Operations { get; set; } = Array.Empty<MemberListOperation>();
}

/// <summary>
/// A group header in a member list.
/// </summary>
public class MemberListGroup
{
    /// <summary>
    /// A hoisted role ID, <c>online</c> or <c>offline</c>.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The number of members in the group.
    /// </summary>
    [JsonProperty("count")]
    public int Count { get; set; }
}

/// <summary>
/// Replaces one range of a member list.
/// </summary>
public class MemberListOperation
{
    /// <summary>
    /// The kind of operation, always <c>SYNC</c>.
    /// </summary>
    [JsonProperty("op")]
    public string Op { get; set; } = string.Empty;

    /// <summary>
    /// The inclusive <c>[start, end]</c> range this operation replaces.
    /// </summary>
    [JsonProperty("range")]
    public int[] Range { get; set; } = Array.Empty<int>();

    /// <summary>
    /// The items that replace the range.
    /// </summary>
    [JsonProperty("items")]
    public MemberListItem[] Items { get; set; } = Array.Empty<MemberListItem>();
}

/// <summary>
/// One position in a member list: either a group header or a member.
/// </summary>
public class MemberListItem
{
    /// <summary>
    /// The group header at this position, or null when it holds a member.
    /// </summary>
    [JsonProperty("group")]
    public MemberListGroup? Group { get; set; }

    /// <summary>
    /// The member at this position, or null when it holds a group header.
    /// </summary>
    [JsonProperty("member")]
    public MemberListMember? Member { get; set; }
}

/// <summary>
/// A guild member in a member list, with their presence.
/// </summary>
public class MemberListMember : GuildMemberJson
{
    /// <summary>
    /// The member's presence. Its status is <c>offline</c> when they aren't visibly online.
    /// </summary>
    [JsonProperty("presence")]
    public PresenceGatewayData? Presence { get; set; }
}
