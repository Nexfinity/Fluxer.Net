using Fluxer.Net.Gateway.Data;

namespace Fluxer.Net;

public interface IMessage
{
    ulong Id { get; }

    ulong ChannelId { get; }

    UserPartialResponse Author { get; }

    ulong? WebhookId { get; }

    MessageType Type { get; }

    MessageFlags Flags { get; }

    string Content { get; }

    DateTime Timestamp { get; }

    DateTime? EditedTimestamp { get; }

    bool Pinned { get; }

    bool MentionEveryone { get; }

    bool Tts { get; }

    UserPartialResponse[]? Mentions { get; }

    ulong[]? MentionRoles { get; }

    MessageEmbedJson[]? Embeds { get; }

    MessageAttachmentJson[]? Attachments { get; }

    MessageStickerJson[]? Stickers { get; }

    MessageReactionResponse[]? Reactions { get; }

    MessageReferenceResponse? MessageReference { get; }

    MessageSnapshotResponse[]? MessageSnapshots { get; }

    string? Nonce { get; }

    MessageCallJson? Call { get; }
}
