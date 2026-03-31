namespace Fluxer.Net;

public class GuildChannel : Channel, IGuildChannel
{
    internal GuildChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static GuildChannel Create(FluxerBaseClient client, ChannelJson json)
    {
        var data = new GuildChannel(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, ChannelJson json)
    {
        base.Update(client, json);
    }
}
