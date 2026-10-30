using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A forum post to count unread messages in, sent with <see cref="FluxerGatewayClient.RequestForumUnreads"/>.
/// </summary>
public class ForumUnreadRequest
{
    /// <summary>
    /// The ID of the post.
    /// </summary>
    [JsonProperty("thread_id")]
    public ulong ThreadId { get; set; }

    /// <summary>
    /// The newest message already read in the post. Messages after it are counted.
    /// </summary>
    [JsonProperty("ack_message_id")]
    public ulong? AckMessageId { get; set; }
}

internal class ForumUnreadsRequestPacket
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("threads")]
    public IEnumerable<ForumUnreadRequest> Threads { get; set; } = Array.Empty<ForumUnreadRequest>();
}
