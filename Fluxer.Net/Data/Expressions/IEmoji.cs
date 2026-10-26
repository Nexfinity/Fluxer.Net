namespace Fluxer.Net;

public interface IEmoji
{
    /// <inheritdoc cref="ISnowflake.Id" />
    ulong? Id { get; }

    /// <inheritdoc cref="ISnowflake.CreatedAt" />
    DateTimeOffset? CreatedAt { get; }

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
