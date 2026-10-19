namespace Fluxer.Net;

public class SocketMediaChannel : MediaChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketMediaChannel(FluxerBaseClient client) : base(client)
    {

    }

}