using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageRequestSchemas.tsx#L247"/>
/// </remarks>
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
    public RichEmbedRequest[]? Embeds { get; set; }

    /// <summary>
    /// Array of attachment objects
    /// </summary>
    [JsonProperty("attachments")]
    public ClientAttachmentRequest[]? Attachments { get; set; }

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
    [MinLength(ApiLimits.MessageNonceMinLength)]
    [MaxLength(ApiLimits.MessageNonceMaxLength)]
    public string Nonce { get; set; }

    /// <summary>
    /// ID of a favorite meme to attach
    /// </summary>
    [JsonProperty("favorite_meme_id")]
    public ulong? FavoriteMemeId { get; set; }

    /// <summary>
    /// Array of sticker IDs to include (max 3)
    /// </summary>
    [JsonProperty("sticker_ids")]
    [MaxLength(3)]
    public HashSet<ulong>? StickerIds { get; set; }

    /// <summary>
    /// Whether this is a text-to-speech message
    /// </summary>
    [JsonProperty("tts")]
    public bool? Tts { get; set; }
}
