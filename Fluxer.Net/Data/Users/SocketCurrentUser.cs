namespace Fluxer.Net;

/// <summary>
/// The current user as the gateway keeps it, with the session and the guilds the client has cached.
/// </summary>
public class SocketCurrentUser : CurrentUser
{
    internal SocketCurrentUser(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// The id of the gateway session.
    /// </summary>
    public string SessionId { get; internal set; }

    /// <summary>
    /// The guilds the client has cached.
    /// </summary>
    public IReadOnlyCollection<SocketGuild> Guilds => (IReadOnlyCollection<SocketGuild>)(Client as FluxerClient).Gateway.Guilds.Values;

    internal SocketCurrentUser Clone() => MemberwiseClone() as SocketCurrentUser;

    /// <summary>
    /// Creates a current user from its JSON data.
    /// </summary>
    /// <param name="client">The client the user belongs to.</param>
    /// <param name="json">The user data.</param>
    /// <returns>The current user.</returns>
    public static new SocketCurrentUser Create(FluxerBaseClient client, UserJson json)
    {
        SocketCurrentUser data = new SocketCurrentUser(client);
        data.Update(json);
        return data;
    }
}