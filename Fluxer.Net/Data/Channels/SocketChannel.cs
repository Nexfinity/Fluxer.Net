namespace Fluxer.Net;

public class SocketChannel : Channel
{
    internal SocketChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketChannel Create(FluxerBaseClient client, ChannelJson json, ulong guildId)
    {
        SocketChannel data = new SocketChannel(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, ChannelJson json)
    {
        base.Update(client, json);
    }
}