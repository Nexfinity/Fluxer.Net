namespace Fluxer.Net;

public class SocketUser : User
{
    internal SocketUser(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketUser object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static new SocketUser Create(FluxerBaseClient client, UserJson json)
    {
        var data = new SocketUser(client);
        data.Update(json);
        return data;
    }
}
