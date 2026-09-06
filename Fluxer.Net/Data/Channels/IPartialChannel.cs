namespace Fluxer.Net;

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
    /// Can you send messages in this channel.
    /// </summary>
    bool IsTextable { get; }
}
