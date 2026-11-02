namespace Fluxer.Net;

/// <summary>
/// A sticker and its basic details.
/// </summary>
public interface ISticker : ISnowflake
{
    /// <summary>
    /// The name of the sticker.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether this sticker is animated.
    /// </summary>
    bool IsAnimated { get; }

    /// <summary>
    /// Whether this sticker can be cloned.
    /// </summary>
    bool AllowCloning { get; }
}
