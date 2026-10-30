using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The answer to <see cref="FluxerGatewayClient.RequestForumUnreads"/>.
/// </summary>
public class ForumUnreadsGatewayData
{
    /// <summary>
    /// The ID of the guild.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The ID of the forum or media channel.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// One entry for each requested post found in the channel.
    /// </summary>
    [JsonProperty("threads")]
    public ForumUnread[] Threads { get; set; } = Array.Empty<ForumUnread>();
}

/// <summary>
/// The unread count of one forum post.
/// </summary>
public class ForumUnread
{
    /// <summary>
    /// The ID of the post.
    /// </summary>
    [JsonProperty("thread_id")]
    public ulong ThreadId { get; set; }

    /// <summary>
    /// Messages after the requested message, at most 25, or null when <see cref="Missing"/> is true.
    /// </summary>
    [JsonProperty("count")]
    public int? Count { get; set; }

    /// <summary>
    /// True when the request had no valid message to count from for this post.
    /// </summary>
    [JsonProperty("missing")]
    public bool Missing { get; set; }
}
