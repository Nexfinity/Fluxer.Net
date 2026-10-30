using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The full member list of a thread the session subscribed to with <see cref="GuildSubscription.ThreadMemberLists"/>.
/// </summary>
public class ThreadMemberListUpdateGatewayData
{
    /// <summary>
    /// The ID of the guild.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The ID of the thread.
    /// </summary>
    [JsonProperty("thread_id")]
    public ulong ThreadId { get; set; }

    /// <summary>
    /// The members of the thread, at most 1,000.
    /// </summary>
    [JsonProperty("members")]
    public ThreadMemberListItem[] Members { get; set; } = Array.Empty<ThreadMemberListItem>();
}

/// <summary>
/// A member of a thread member list.
/// </summary>
public class ThreadMemberListItem
{
    /// <summary>
    /// The ID of the user.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// When the user joined the thread.
    /// </summary>
    [JsonProperty("join_timestamp")]
    public DateTimeOffset JoinedAt { get; set; }

    /// <summary>
    /// The thread member flags.
    /// </summary>
    [JsonProperty("flags")]
    public int Flags { get; set; }

    /// <summary>
    /// The user's guild member.
    /// </summary>
    [JsonProperty("member")]
    public GuildMemberJson? Member { get; set; }

    /// <summary>
    /// The user's presence. Its status is <c>offline</c> when they aren't visibly online.
    /// </summary>
    [JsonProperty("presence")]
    public PresenceGatewayData? Presence { get; set; }
}
