using Fluxer.Net.Data.Channels;

namespace Fluxer.Net;

internal interface IChannelPins
{
    /// <summary>
    /// Pinned messages in this channel
    /// </summary>
    IEnumerable<IChannelPin> Items { get; }

    /// <summary>
    /// Whether more pins can be fetched with pagination
    /// </summary>
    bool HasMore { get; }
}
