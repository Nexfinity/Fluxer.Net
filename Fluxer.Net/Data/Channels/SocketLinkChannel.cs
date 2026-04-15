namespace Fluxer.Net;

public class SocketLinkChannel : LinkChannel
{
    internal SocketLinkChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketLinkChannel Create(FluxerBaseClient client, ChannelJson json)
    {
        var data = new SocketLinkChannel(client);
        data.Update(client, json);
        return data;
    }
}
