namespace Fluxer.Net;

/// <inheritdoc />
public class SocketCurrentUser : CurrentUser
{
    internal SocketCurrentUser(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Cached guilds for the current user.
    /// </summary>
    public IReadOnlyCollection<SocketGuild> Guilds => (IReadOnlyCollection<SocketGuild>)(Client as FluxerClient).Gateway.Guilds.Values;

    /// <summary>
    /// Create a SocketCurrentUser object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static new SocketCurrentUser Create(FluxerBaseClient client, UserJson json)
    {
        SocketCurrentUser data = new SocketCurrentUser(client);
        data.Update(json);
        return data;
    }
}