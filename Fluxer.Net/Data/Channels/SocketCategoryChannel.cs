namespace Fluxer.Net;

public class SocketCategoryChannel : CategoryChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketCategoryChannel(FluxerBaseClient client) : base(client)
    {

    }
}
