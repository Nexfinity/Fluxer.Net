namespace Fluxer.Net;

public class SocketNewsChannel : SocketTextChannel, INewsChannel
{
    internal SocketNewsChannel(FluxerBaseClient client) : base(client)
    {

    }
}