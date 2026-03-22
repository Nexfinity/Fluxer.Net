using Fluxer.Net.Data.Channels;

namespace Fluxer.Net;

/// <summary>
/// List of pinned messages as pagination.
/// </summary>
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
