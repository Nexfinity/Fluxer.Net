namespace Fluxer.Net;

/// <inheritdoc />
public class SocketUser : User
{
    internal SocketUser(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketUser object from json.
    /// </summary>
    /// <returns><see cref="SocketUser"/></returns>
    public static new SocketUser Create(FluxerBaseClient client, UserJson json)
    {
        SocketUser data = new SocketUser(client);
        data.Update(json);
        return data;
    }
}