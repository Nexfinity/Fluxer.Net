namespace Fluxer.Net;

public class SocketCategoryChannel : CategoryChannel
{
    internal SocketCategoryChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketCategoryChannel Create(FluxerBaseClient client, ChannelJson json)
    {
        var data = new SocketCategoryChannel(client);
        data.Update(client, json);
        return data;
    }
}
