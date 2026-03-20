using Fluxer.Net.Gateway.Data;

namespace Fluxer.Net;

/// <inheritdoc />
public class Message : Entity, IMessage
{
    /// <inheritdoc />
    public ulong Id { get; set; }

    /// <inheritdoc />
    public ulong ChannelId { get; set; }

    /// <inheritdoc />
    public UserPartialResponse Author { get; set; }

    /// <inheritdoc />
    public ulong? WebhookId { get; set; }

    /// <inheritdoc />
    public MessageType Type { get; set; }

    /// <inheritdoc />
    public MessageFlags Flags { get; set; }

    /// <inheritdoc />
    public string Content { get; set; }

    /// <inheritdoc />
    public DateTime Timestamp { get; set; }

    /// <inheritdoc />
    public DateTime? EditedTimestamp { get; set; }

    /// <inheritdoc />
    public bool Pinned { get; set; }

    /// <inheritdoc />
    public bool MentionEveryone { get; set; }

    /// <inheritdoc />
    public bool Tts { get; set; }

    /// <inheritdoc />
    public UserPartialResponse[]? Mentions { get; set; }

    /// <inheritdoc />
    public ulong[]? MentionRoles { get; set; }

    /// <inheritdoc />
    public EmbedJson[]? Embeds { get; set; }

    /// <inheritdoc />
    public MessageAttachmentJson[]? Attachments { get; set; }

    /// <inheritdoc />
    public MessageStickerJson[]? Stickers { get; set; }

    /// <inheritdoc />
    public MessageReactionResponse[]? Reactions { get; set; }

    /// <inheritdoc />
    public MessageReferenceResponse? MessageReference { get; set; }

    /// <inheritdoc />
    public MessageSnapshotResponse[]? MessageSnapshots { get; set; }

    /// <inheritdoc />
    public string? Nonce { get; set; }

    /// <inheritdoc />
    public MessageCallJson? Call { get; set; }

    internal Message(BaseClient client) : base(client)
    {

    }

    public static Message Create(BaseClient client, MessageJson json)
    {
        var data = new Message(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, MessageJson json)
    {
        Id = json.Id;
        ChannelId = json.ChannelId;
        Author = json.Author;
        WebhookId = json.WebhookId;
        Type = json.Type;
        Flags = json.Flags;
        Content = json.Content;
        Timestamp = json.Timestamp;
        EditedTimestamp = json.EditedTimestamp;
        Pinned = json.Pinned;
        MentionEveryone = json.MentionEveryone;
        Tts = json.Tts;
        Mentions = json.Mentions;
        MentionRoles = json.MentionRoles;
        Embeds = json.Embeds;
        Attachments = json.Attachments;
        Stickers = json.Stickers;
        Reactions = json.Reactions;
        MessageReference = json.MessageReference;
        MessageSnapshots = json.MessageSnapshots;
        Nonce = json.Nonce;
        Call = json.Call;
    }
}
