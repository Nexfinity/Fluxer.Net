namespace Fluxer.Net;

public class SocketTextChannel : TextChannel, ISocketGuildChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketTextChannel(FluxerBaseClient client) : base(client)
    {

    }
}
