namespace Fluxer.Net;

public interface IGuildEmoji : IEmoji
{
    /// <summary>
    /// The user that created the emoji.
    /// </summary>
    IUser? Creator { get; }

    /// <summary>
    /// Get the emoji's image.
    /// </summary>
    string GetEmojiUrl(int size);
}
