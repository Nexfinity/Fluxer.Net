using System.Collections.Concurrent;

namespace Fluxer.Net;

public class SocketGuildMember : GuildMember
{
    public SocketGuild Guild { get; internal set; }

    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; internal set; } = new ConcurrentDictionary<string, SocketVoiceState>();

    public IEnumerable<SocketRole> Roles
            => RoleIds.Select(id => Guild.Roles[id]).Where(x => x != null);

    public bool HasPermission(Permissions permission)
    {
        foreach (var r in Roles)
        {
            if (r.Permissions.RawValue.HasFlag(permission))
                return true;
        }

        return false;
    }

    internal SocketGuildMember(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketGuildMember Create(FluxerBaseClient client, GuildMemberJson json)
    {
        SocketGuildMember data = new SocketGuildMember(client);
        data.Update(client, json);
        return data;
    }

    internal override void Update(FluxerBaseClient client, GuildMemberJson json)
    {
        base.Update(client, json);
    }
}