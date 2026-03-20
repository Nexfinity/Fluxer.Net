namespace Fluxer.Net.Data.Channels;

public interface IChannelPin
{
    /// <summary>
    /// The message data for this pin.
    /// </summary>
    IMessage Message { get; }

    /// <summary>
    /// The ISO 8601 timestamp of when the message was pinned
    /// </summary>
    DateTime PinnedAt { get; }
}
