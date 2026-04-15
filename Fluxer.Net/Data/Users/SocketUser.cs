namespace Fluxer.Net;

public class SocketUser : User
{
    internal SocketUser(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketUser Create(FluxerBaseClient client, UserJson json)
    {
        var data = new SocketUser(client);
        data.Update(json);
        return data;
    }
}
