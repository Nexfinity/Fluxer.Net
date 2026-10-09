namespace Fluxer.Net;

public interface IForumTag
{
    ulong Id { get; }

    string Name { get; }

    ulong? EmojiId { get; }

    string? EmojiName { get; }

    bool IsModerated { get; }
}
