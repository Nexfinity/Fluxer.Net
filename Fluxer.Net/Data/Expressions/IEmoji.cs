namespace Fluxer.Net;

public interface IEmoji
{
    /// <summary>
    /// The unique identifier for this emoji.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The name of the emoji
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether this emoji is animated.
    /// </summary>
    bool IsAnimated { get; }
}
