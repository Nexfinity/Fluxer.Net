namespace Fluxer.Net;

public interface ISticker
{
    /// <summary>
    /// The unique identifier for this sticker.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The name of the sticker.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether this sticker is animated.
    /// </summary>
    bool IsAnimated { get; }

    /// <summary>
    /// Wether you can clone this emoji.
    /// </summary>
    bool AllowCloning { get; }
}
