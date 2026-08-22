namespace Fluxer.Net;

public interface IMessageSnapshot
{
    /// <summary>
    /// The text content of the snapshot.
    /// </summary>
    string? Content { get; }

    /// <summary>
    /// The ISO 8601 timestamp of when the original message was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// The ISO 8601 timestamp of when the original message was last edited
    /// </summary>
    DateTimeOffset? EditedAt { get; }

    /// <summary>
    /// The user IDs mentioned in the snapshot.
    /// </summary>
    HashSet<ulong>? MentionedUserIds { get; }

    /// <summary>
    /// The role IDs mentioned in the snapshot.
    /// </summary>
    HashSet<ulong>? MentionedRoleIds { get; }

    /// <summary>
    /// The embeds included in the snapshot.
    /// </summary>
    IEmbed[]? Embeds { get; }

    /// <summary>
    /// The attachments included in the snapshot.
    /// </summary>
    IAttachment[]? Attachments { get; }

    /// <summary>
    /// The stickers included in the snapshot.
    /// </summary>
    ISticker[]? Stickers { get; }

    /// <summary>
    /// The type of message.
    /// </summary>
    MessageType Type { get; }

    /// <summary>
    /// Message flags bitfield.
    /// </summary>
    MessageFlag Flags { get; }
}
