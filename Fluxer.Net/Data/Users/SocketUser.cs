namespace Fluxer.Net;

/// <summary>
/// A user as the gateway keeps it.
/// </summary>
public class SocketUser : User
{
    internal SocketUser(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a user from its JSON data.
    /// </summary>
    /// <param name="client">The client the user belongs to.</param>
    /// <param name="json">The user data.</param>
    /// <returns>The user.</returns>
    public static new SocketUser Create(FluxerBaseClient client, UserJson json)
    {
        SocketUser data = new SocketUser(client);
        data.Update(json);
        return data;
    }
}