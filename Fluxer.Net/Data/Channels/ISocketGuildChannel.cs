namespace Fluxer.Net;

/// <summary>
/// A cached guild channel that knows its <see cref="SocketGuild"/>.
/// </summary>
public interface ISocketGuildChannel : IGuildChannel
{
    /// <summary>
    /// The cached guild this channel belongs to.
    /// </summary>
    SocketGuild Guild { get; }
}
