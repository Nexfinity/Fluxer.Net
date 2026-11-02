namespace Fluxer.Net;

/// <summary>
/// A guild role cached from gateway events.
/// </summary>
public class SocketRole : Role
{
    /// <summary>
    /// The guild the role is part of.
    /// </summary>
    public SocketGuild Guild { get; private set; }

    /// <summary>
    /// Whether the role, or the guild's @everyone role, grants the permission.
    /// </summary>
    /// <param name="permission">The permission to check.</param>
    public bool HasPermission(GuildPermission permission)
    {
        if (Permissions.RawValue.HasFlag(permission))
            return true;

        if (Guild.Permissions.RawValue.HasFlag(permission))
            return true;

        return false;
    }

    internal SocketRole Clone() => MemberwiseClone() as SocketRole;

    internal SocketRole(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a cached role from its JSON representation in the given guild.
    /// </summary>
    /// <returns><see cref="SocketRole"/></returns>
    public static SocketRole Create(FluxerBaseClient client, RoleJson json, SocketGuild guild)
    {
        SocketRole data = new SocketRole(client)
        {
            GuildId = guild.Id,
            Guild = guild
        };
        data.Update(json);
        return data;
    }

    internal override void Update(RoleJson json)
    {
        base.Update(json);
    }
}
