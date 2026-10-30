using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class MessageReferenceRequest
{
    /// <summary>
    /// ID of the message being referenced
    /// </summary>
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    /// <summary>
    /// ID of the channel containing the referenced message
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// ID of the guild containing the referenced message
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <summary>
    /// Type of reference
    /// </summary>
    [JsonProperty("type")]
    public MessageReferenceType Type { get; set; }

    /// <summary>
    /// When forwarding, only these attachments of the original message are included.
    /// </summary>
    [JsonProperty("attachment_ids")]
    public ulong[]? AttachmentIds { get; set; }

    /// <summary>
    /// When forwarding, only the embeds at these positions of the original message are included.
    /// </summary>
    [JsonProperty("embed_indices")]
    public int[]? EmbedIndices { get; set; }
}
