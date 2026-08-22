namespace Fluxer.Net;

public interface IEmoji : ISnowflake
{
    /// <summary>
    /// The name of the emoji
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether this emoji is animated.
    /// </summary>
    bool IsAnimated { get; }

    /// <summary>
    /// Wether you can clone this emoji.
    /// </summary>
    bool AllowCloning { get; }
}
