namespace Fluxer.Net;

/// <summary>
/// A cached guild announcement channel.
/// </summary>
public class SocketNewsChannel : SocketTextChannel, INewsChannel
{
    internal SocketNewsChannel(FluxerBaseClient client) : base(client)
    {

    }
}