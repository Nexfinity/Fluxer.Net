namespace Fluxer.Net.Data.Messages;

/// <summary>
/// Represents the format/type of a sticker.
/// </summary>
public enum StickerFormatType
{
    /// <summary>
    /// Static PNG image sticker.
    /// </summary>
    Png = 1,

    /// <summary>
    /// Animated PNG sticker.
    /// </summary>
    Apng = 2,

    /// <summary>
    /// Lottie (JSON-based) animation sticker.
    /// </summary>
    Lottie = 3,

    /// <summary>
    /// GIF animation sticker.
    /// </summary>
    Gif = 4,
}
