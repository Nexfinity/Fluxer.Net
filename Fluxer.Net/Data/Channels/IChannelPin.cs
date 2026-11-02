namespace Fluxer.Net;

/// <summary>
/// A message pinned in a channel.
/// </summary>
public interface IChannelPin
{
    /// <summary>
    /// The pinned message.
    /// </summary>
    IMessage Message { get; }

    /// <summary>
    /// The time the message was pinned.
    /// </summary>
    DateTimeOffset PinnedAt { get; }
}
