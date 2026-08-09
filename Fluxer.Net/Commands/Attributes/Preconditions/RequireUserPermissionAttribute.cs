namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireUserPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// Server/group permission to check for.
    /// </summary>
    private GuildPermission? Server;

    /// <summary>
    /// Channel permission to check for.
    /// </summary>
    private ChannelPermission? Channel;

    public RequireUserPermissionAttribute(GuildPermission perm)
    {
        Server = perm;
    }

    public RequireUserPermissionAttribute(ChannelPermission perm)
    {
        Channel = perm;
    }

    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Guild == null)
            return PreconditionResult.FromError("You need to run this command in a community.");

        SocketGuildMember? member = context.Member as SocketGuildMember;

        if (Server.HasValue)
        {
            if (member != null && member.HasPermission(Server.Value))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError($"You need community permission for **{Server.Value.ToString()}** to use this command.");
        }

        if (Channel == null)
            return PreconditionResult.FromError($"Invalid command precondition for RequireUserPermission.");

        ChannelPermissions perms = member.GetPermissions(context.Channel as Channel);
        if (perms.RawValue.HasFlag(Channel.Value))
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError($"You need channel permission for **{Channel.Value.ToString()}** to use this command.");
    }
}