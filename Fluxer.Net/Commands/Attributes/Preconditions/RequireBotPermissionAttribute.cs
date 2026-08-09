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
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Guild == null || context.Guild is not SocketGuild guild)
            return PreconditionResult.FromError("You need to run this command in a server.");

        SocketGuildMember? member = guild.CurrentMember as SocketGuildMember;

        if (Server.HasValue)
        {
            if (member != null && member.HasPermission(Server.Value))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError($"Bot needs server permission for **{Server.Value.ToString()}** to use this command.");
        }

        if (Channel == null)
            return PreconditionResult.FromError($"Invalid command precondition for RequireBotPermission.");

        ChannelPermissions perms = member.GetPermissions(context.Channel as Channel);
        if (perms.RawValue.HasFlag(Channel.Value))
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError($"Bot needs channel permission for **{Channel.Value.ToString()}** to use this command.");
    }
}