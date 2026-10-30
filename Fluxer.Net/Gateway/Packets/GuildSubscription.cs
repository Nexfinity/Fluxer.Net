using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// What the session wants to receive from one guild, sent with <see cref="FluxerGatewayClient.SendLazyRequest"/>.
/// Options left null are not sent and keep their current value.
/// </summary>
public class GuildSubscription
{
    /// <summary>
    /// Whether the guild is active for this session. Bot sessions are always active.
    /// </summary>
    [JsonProperty("active")]
    public bool? Active { get; set; }

    /// <summary>
    /// Whether to ask for a guild sync.
    /// </summary>
    [JsonProperty("sync")]
    public bool? Sync { get; set; }

    /// <summary>
    /// Whether the guild sends typing events.
    /// </summary>
    [JsonProperty("typing")]
    public bool? Typing { get; set; }

    /// <summary>
    /// The members whose presence updates the session receives, at most 1,000. Replaces the previous list for this guild.
    /// Without this, a session gets no presence updates for the guild's members.
    /// </summary>
    [JsonProperty("members")]
    public IEnumerable<ulong>? Members { get; set; }

    /// <summary>
    /// Member list windows to subscribe to, keyed by channel ID. Each range is <c>[start, end]</c> with at most 100 positions.
    /// Raises <see cref="FluxerGatewayClient.GuildMemberListUpdated"/>.
    /// </summary>
    [JsonProperty("member_list_channels")]
    public IDictionary<ulong, IEnumerable<int[]>>? MemberListChannels { get; set; }

    /// <summary>
    /// The channel or public thread open in a guild the session is visiting.
    /// </summary>
    [JsonProperty("focused_channel_id")]
    public ulong? FocusedChannelId { get; set; }

    /// <summary>
    /// Whether the session gets events and messages for every thread it can view in the guild.
    /// </summary>
    [JsonProperty("threads")]
    public bool? Threads { get; set; }

    /// <summary>
    /// The threads whose member lists the session subscribes to, at most 10. Replaces the previous list, and an empty list clears it.
    /// Raises <see cref="FluxerGatewayClient.ThreadMemberListUpdated"/>.
    /// </summary>
    [JsonProperty("thread_member_lists")]
    public IEnumerable<ulong>? ThreadMemberLists { get; set; }
}

internal class LazyRequestPacket
{
    [JsonProperty("subscriptions")]
    public IDictionary<ulong, GuildSubscription> Subscriptions { get; set; } = new Dictionary<ulong, GuildSubscription>();
}
