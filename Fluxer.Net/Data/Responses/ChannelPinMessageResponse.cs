using Fluxer.Net.Data.Enums;
using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L179C14-L179C39"/>
/// everything but ReferencedMessages, and Reactions
/// </remarks>
public class ChannelPinMessageResponse
{
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonRequired]
    [JsonProperty("author")]
    public UserPartialResponse Author { get; set; }

    [JsonProperty("webhook_id")]
    public ulong? WebhookId { get; set; }

    [JsonProperty("type")]
    public MessageType Type { get; set; }

    [JsonProperty("flags")]
    public MessageFlags Flags { get; set; }

    [JsonRequired]
    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("edited_timestamp")]
    public DateTime? EditedTimestamp { get; set; }

    [JsonProperty("pinned")]
    public bool Pinned { get; set; }

    [JsonProperty("mention_everyone")]
    public bool MentionEveryone { get; set; }

    [JsonProperty("tts")]
    public bool Tts { get; set; }

    [JsonProperty("mentions")]
    public UserPartialResponse[]? Mentions { get; set; }

    [JsonProperty("mention_roles")]
    public ulong[]? MentionRoles { get; set; }

    [JsonProperty("embeds")]
    public MessageEmbedResponse[]? Embeds { get; set; }

    [JsonProperty("attachments")]
    public MessageAttachmentResponse[]? Attachments { get; set; }

    [JsonProperty("stickers")]
    public MessageStickerResponse[]? Stickers { get; set; }

    [JsonProperty("message_snapshots")]
    public MessageSnapshotResponse[]? MessageSnapshots { get; set; }

    [JsonProperty("nonce")]
    public string? Nonce { get; set; }

    [JsonProperty("call")]
    public MessageCallResponse? Call { get; set; }
}
