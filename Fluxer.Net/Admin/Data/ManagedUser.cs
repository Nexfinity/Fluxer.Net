namespace Fluxer.Net.Admin;

/// <summary>
/// A user read through a <see cref="FluxerAdminClient"/>.
/// </summary>
public class ManagedUser : CurrentUser
{
    internal ManagedUser(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a user from its API data.
    /// </summary>
    /// <param name="client">The admin client the user was read with.</param>
    /// <param name="json">The user data.</param>
    public static ManagedUser Create(FluxerAdminClient client, CurrentUserJson json)
    {
        ManagedUser data = new ManagedUser(client);
        data.Update(json);
        return data;
    }
}
