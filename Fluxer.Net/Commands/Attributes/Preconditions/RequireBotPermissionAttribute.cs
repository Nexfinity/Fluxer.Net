namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// Server permission to check for.
    /// </summary>
    private GuildPermission? Server;

    /// <summary>
    /// Channel permission to check for.
    /// </summary>
    private ChannelPermission? Channel;

    public RequireBotPermissionAttribute(GuildPermission perm)
    {
        Server = perm;
    }

    public RequireBotPermissionAttribute(ChannelPermission perm)
    {
        Channel = perm;
    }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Server == null)
            return Task.FromResult(PreconditionResult.FromError("You need to run this command in a server."));

        if (Server.HasValue)
        {
            if (context.Server.CurrentMember.HasPermission(Server.Value))
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError($"Bot needs server permission for **{Server.Value.ToString()}** to use this command."));
        }

        if (Channel == null)
            return Task.FromResult(PreconditionResult.FromError($"Invalid command precondition for RequireBotPermission."));

        ChannelPermissions perms = context.Server.CurrentMember.GetPermissions(context.Channel);
        if (perms.RawValue.HasFlag(Channel.Value))
            return Task.FromResult(PreconditionResult.FromSuccess());

        return Task.FromResult(PreconditionResult.FromError($"Bot needs channel permission for **{Channel.Value.ToString()}** to use this command."));
    }
}