namespace Fluxer.Net;

/// <summary>
/// A channel with its ID, name and type.
/// </summary>
public interface IPartialChannel : ISnowflake, IMentionable
{
    /// <summary>
    /// The name of the channel.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// The type of the channel.
    /// </summary>
    ChannelType Type { get; }

    /// <summary>
    /// Whether messages can be sent in this channel.
    /// </summary>
    bool IsTextable { get; }
}
