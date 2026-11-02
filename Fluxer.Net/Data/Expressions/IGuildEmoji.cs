namespace Fluxer.Net;

/// <summary>
/// A custom emoji that belongs to a guild.
/// </summary>
public interface IGuildEmoji : IEmoji
{
    /// <summary>
    /// The user that created the emoji.
    /// </summary>
    IUser? Creator { get; }

    /// <summary>
    /// Gets the URL of the emoji's image.
    /// </summary>
    string GetEmojiUrl(int size);
}
