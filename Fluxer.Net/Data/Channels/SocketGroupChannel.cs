namespace Fluxer.Net;

public class SocketGroupChannel : GroupChannel
{
    internal SocketGroupChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketGroupChannel Create(FluxerBaseClient client, ChannelJson json)
    {
        var data = new SocketGroupChannel(client);
        data.Update(client, json);
        return data;
    }
}
