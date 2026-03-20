namespace Fluxer.Net;

public class Channel : Entity
{
    internal Channel(BaseClient client) : base(client)
    {

    }

    public static Channel Create(BaseClient client, ChannelJson json)
    {
        var data = new Channel(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, ChannelJson json)
    {

    }
}
