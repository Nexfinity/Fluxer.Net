namespace Fluxer.Net;

public interface IMessageSnapshot
{
    string? Content { get; }

    DateTime CreatedAt { get; }

    DateTime? EditedAt { get; }

    HashSet<ulong>? MentionedUserIds { get; }

    HashSet<ulong>? MentionedRoleIds { get; }

    IEmbed[]? Embeds { get; }

    IAttachment[]? Attachments { get; }

    ISticker[]? Stickers { get; }

    MessageType Type { get; }

    MessageFlag Flags { get; }
}
