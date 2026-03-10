using Fluxer.Net.Data.Voice;
using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Messages;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L129"/>
/// </remarks>
public class MessageBaseResponse
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
    
    [JsonProperty("reactions")]
    public MessageReactionResponse[]? Reactions { get; set; }
    
    [JsonProperty("message_reference")]
    public MessageReferenceResponse? MessageReference { get; set; }
    
    [JsonProperty("message_snapshots")]
    public MessageSnapshotResponse[]? MessageSnapshots { get; set; }
    
    [JsonProperty("nonce")]
    public string? Nonce { get; set; }
    
    [JsonProperty("call")]
    public MessageCallResponse? Call { get; set; }
}
