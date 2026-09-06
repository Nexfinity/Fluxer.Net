namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireUserPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// Guild/Group permission to check for.
    /// </summary>
    private readonly GuildPermission? Guild;

    /// <summary>
    /// Channel permission to check for.
    /// </summary>
    private readonly ChannelPermission? Channel;

    public RequireUserPermissionAttribute(GuildPermission perm)
    {
        Guild = perm;
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

        if (context.CommandService._ownerBypassPermissions)
        {
            if (context.CommandService._ownerIds?.Any(x => x == context.User.Id) == true)
                return PreconditionResult.FromSuccess();

            ulong? OwnerId = await context.Gateway.GetOwnerIdAsync();
            if (OwnerId.HasValue && OwnerId.Value == context.User.Id)
                return PreconditionResult.FromSuccess();
        }

        SocketGuildMember? member = context.Member as SocketGuildMember;

        if (Guild.HasValue)
        {
            if (member != null && member.HasPermission(Guild.Value))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError($"You need community permission for **{Guild.Value.ToString()}** to use this command.");
        }

        if (Channel == null)
            return PreconditionResult.FromError($"Invalid command precondition for RequireUserPermission.");

        ChannelPermissions perms = member.GetPermissions(context.Channel as Channel);
        if (perms.RawValue.HasFlag(Channel.Value))
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError($"You need channel permission for **{Channel.Value.ToString()}** to use this command.");
    }
}