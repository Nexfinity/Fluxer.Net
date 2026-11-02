namespace Fluxer.Net.Admin;

/// <summary>
/// A guild read through a <see cref="FluxerAdminClient"/>.
/// </summary>
public class ManagedGuild : Guild
{
    internal ManagedGuild(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a guild from its API data.
    /// </summary>
    /// <param name="client">The admin client the guild was read with.</param>
    /// <param name="json">The guild data.</param>
    public static ManagedGuild Create(FluxerAdminClient client, GuildJson json)
    {
        ManagedGuild data = new ManagedGuild(client);
        data.Update(json);
        return data;
    }
}
