namespace Fluxer.Net;

public class SocketCategoryChannel : CategoryChannel, ISocketGuildChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketCategoryChannel(FluxerBaseClient client) : base(client)
    {

    }
}
