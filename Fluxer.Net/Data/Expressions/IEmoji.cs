namespace Fluxer.Net;

/// <summary>
/// An emoji, either custom to a guild or a unicode emoji.
/// </summary>
public interface IEmoji
{
    /// <inheritdoc cref="ISnowflake.Id" />
    ulong? Id { get; }

    /// <inheritdoc cref="ISnowflake.CreatedAt" />
    DateTimeOffset? CreatedAt { get; }

    /// <summary>
    /// The name of the emoji.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether this emoji is animated.
    /// </summary>
    bool IsAnimated { get; }

    /// <summary>
    /// Whether this emoji can be cloned.
    /// </summary>
    bool AllowCloning { get; }
}
