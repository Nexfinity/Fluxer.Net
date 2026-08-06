using Fluxer.Net.Rest.Requests;
using Newtonsoft.Json;

namespace Fluxer.Net;

public class UpdateMessageRequest
{
    /// <summary>
    /// The message content (up to 2000 characters)
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Array of embed objects to include in the message
    /// </summary>
    [JsonProperty("embeds")]
    public EmbedRequest[]? Embeds { get; set; }

    /// <summary>
    /// Controls which mentions trigger notifications
    /// </summary>
    [JsonProperty("allowed_mentions")]
    public AllowedMentionsRequest? AllowedMentions { get; set; }

    /// <summary>
    /// Message flags bitfield
    /// </summary>
    [JsonProperty("flags")]
    public MessageFlag? Flags { get; set; }

    /// <summary>
    /// Array of attachment objects to keep or add
    /// </summary>
    [JsonProperty("attachments")]
    public AttachmentRequest[]? Attachments { get; set; }
}
