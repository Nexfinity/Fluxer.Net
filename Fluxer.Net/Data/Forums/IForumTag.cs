namespace Fluxer.Net;

/// <summary>
/// A tag that forum posts can be given.
/// </summary>
public interface IForumTag
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The ID of the custom emoji shown with the tag.
    /// </summary>
    ulong? EmojiId { get; }

    /// <summary>
    /// The name of the emoji shown with the tag.
    /// </summary>
    string? EmojiName { get; }

    /// <summary>
    /// Whether the tag is moderated.
    /// </summary>
    bool IsModerated { get; }
}
