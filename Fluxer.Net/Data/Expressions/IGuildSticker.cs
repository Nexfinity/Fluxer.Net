namespace Fluxer.Net;

public interface IGuildSticker : ISticker
{
    /// <summary>
    /// The description of the sticker.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Autocomplete/suggestion tags for the sticker.
    /// </summary>
    List<string>? Tags { get; }

    /// <summary>
    /// The user that created the sticker.
    /// </summary>
    IUser? Creator { get; }

    /// <summary>
    /// Get the sticker's image.
    /// </summary>
    string GetStickerUrl(int size);
}
