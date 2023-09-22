using System;
using Newtonsoft.Json;
using Squll.Net.Objects.DataTables;

namespace Squll.Net.Objects;

public class Message
{
    [JsonProperty("attachments")]
    public object[] Attachments { get; set; }
    [JsonProperty("author")]
    public User Author { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonProperty("embeds")]
    public object[] Embeds { get; set; }
    [JsonProperty("expires_at")]
    public object? ExpiresAt { get; set; }
    [JsonProperty("flags")]
    public MessageFlags Flags { get; set; }
    [JsonProperty("id")]
    public ulong Id { get; set; }
    [JsonProperty("mention_roles")]
    public object[] MentionRoles { get; set; }
    [JsonProperty("mention_users")]
    public ulong[] MentionUsers { get; set; }
    [JsonProperty("message_reference")]
    public object? MessageReference { get; set; }
    [JsonProperty("nonce")]
    public string Nonce { get; set; }
    [JsonProperty("reactions")]
    public object[] Reactions { get; set; }
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
    [JsonProperty("type")]
    public MessageType Type { get; set; }
    [JsonProperty("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
