namespace Fluxer.Net;

public interface IMessage
{
    /// <summary>
    /// Unique identifier (snowflake) for the object.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The ID of the channel this message was sent in.
    /// </summary>
    ulong ChannelId { get; }

    /// <summary>
    /// Partial user that sent the message.
    /// </summary>
    IUser Author { get; }

    /// <summary>
    /// The ID of the webhook that sent this message.
    /// </summary>
    ulong? WebhookId { get; }

    /// <summary>
    /// The type of message.
    /// </summary>
    MessageType Type { get; }

    /// <summary>
    /// Message flags bitfield.
    /// </summary>
    MessageFlag Flags { get; }

    /// <summary>
    /// The text content of the message.
    /// </summary>
    string Content { get; }

    /// <summary>
    /// The ISO 8601 timestamp of when the message was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// The ISO 8601 timestamp of when the message was last edited.
    /// </summary>
    DateTimeOffset? EditedAt { get; }

    /// <summary>
    /// Whether the message is pinned.
    /// </summary>
    bool IsPinned { get; }

    /// <summary>
    /// Whether the message mentions @everyone.
    /// </summary>
    bool MentionEveryone { get; }

    /// <summary>
    /// Whether the message was sent as text-to-speech.
    /// </summary>
    bool IsTTS { get; }

    /// <summary>
    /// The users mentioned in the message.
    /// </summary>
    IEnumerable<IUser>? Mentions { get; }

    /// <summary>
    /// The role IDs mentioned in the message.
    /// </summary>
    ulong[]? MentionRoles { get; }

    /// <summary>
    /// The embeds attached to the message.
    /// </summary>
    IEnumerable<IEmbed>? Embeds { get; }

    /// <summary>
    /// The files attached to the message.
    /// </summary>
    IAttachment[]? Attachments { get; }

    /// <summary>
    /// The stickers sent with the message.
    /// </summary>
    ISticker[]? Stickers { get; }

    /// <summary>
    /// The reactions on the message
    /// </summary>
    IMessageReaction[]? Reactions { get; }

    /// <summary>
    /// Reference data for replies or forwards.
    /// </summary>
    IMessageReference? MessageReference { get; }

    /// <summary>
    /// Snapshots of forwarded messages.
    /// </summary>
    IMessageSnapshot[]? MessageSnapshots { get; }

    /// <summary>
    /// A client-provided value for message deduplication.
    /// </summary>
    string? Nonce { get; }

    /// <summary>
    /// Call information if this message represents a call
    /// </summary>
    IMessageCall? Call { get; }
}
