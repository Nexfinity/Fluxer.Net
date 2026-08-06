using Fluxer.Net.Rest.Requests;
using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageRequest
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
    /// Array of attachment objects
    /// </summary>
    [JsonProperty("attachments")]
    public List<AttachmentJson>? Attachments { get; set; }

    /// <summary>
    /// Reference to another message (for replies or forwards)
    /// </summary>
    [JsonProperty("message_reference")]
    public MessageReferenceRequest? MessageReference { get; set; }

    /// <summary>
    /// Controls which mentions trigger notifications
    /// </summary>
    [JsonProperty("allowed_mentions")]
    public AllowedMentionsRequest? AllowedMentions { get; set; }

    /// <summary>
    /// Message flags bitfield
    /// </summary>
    [JsonProperty("flags")]
    public MessageFlag Flags { get; set; }

    /// <summary>
    /// Client-generated identifier for the message
    /// </summary>
    [JsonProperty("nonce")]
    public string? Nonce { get; set; }

    /// <summary>
    /// ID of a favorite meme to attach
    /// </summary>
    [JsonProperty("favorite_meme_id")]
    public ulong? FavoriteMemeId { get; set; }

    /// <summary>
    /// Array of sticker IDs to include (max 3)
    /// </summary>
    [JsonProperty("sticker_ids")]
    public List<ulong>? StickerIds { get; set; }

    /// <summary>
    /// Whether this is a text-to-speech message
    /// </summary>
    [JsonProperty("tts")]
    public bool? Tts { get; set; }
}
