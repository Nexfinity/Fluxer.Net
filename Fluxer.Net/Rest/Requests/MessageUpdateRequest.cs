using Fluxer.Net.Data.Messages;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/api/src/channel/MessageTypes.tsx#L32"/>
/// </remarks>
public class MessageUpdateRequest
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
    /// Controls which mentions trigger notifications
    /// </summary>
    [JsonProperty("allowed_mentions")]
    public AllowedMentionsRequest? AllowedMentions { get; set; }

    /// <summary>
    /// Message flags bitfield
    /// </summary>
    [JsonProperty("flags")]
    public MessageFlags? Flags { get; set; }

    /// <summary>
    /// Array of attachment objects to keep or add
    /// </summary>
    [JsonProperty("attachments")]
    public ClientAttachmentReferenceRequest[]? Attachments { get; set; }
}
