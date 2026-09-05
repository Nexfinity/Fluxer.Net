namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// Guild permission to check for.
    /// </summary>
    private readonly GuildPermission? Guild;

    /// <summary>
    /// Channel permission to check for.
    /// </summary>
    private readonly ChannelPermission? Channel;

    public RequireBotPermissionAttribute(GuildPermission perm)
    {
        Guild = perm;
    }

    public RequireBotPermissionAttribute(ChannelPermission perm)
    {
        Channel = perm;
    }

    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Guild == null || context.Guild is not SocketGuild guild)
            return PreconditionResult.FromError("You need to run this command in a community.");

        SocketGuildMember? member = guild.CurrentMember as SocketGuildMember;

        if (Guild.HasValue)
        {
            if (member != null && member.HasPermission(Guild.Value))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError($"Bot needs community permission for **{Guild.Value.ToString()}** to use this command.");
        }

        if (Channel == null)
            return PreconditionResult.FromError($"Invalid command precondition for RequireBotPermission.");

        ChannelPermissions perms = member.GetPermissions(context.Channel as Channel);
        if (perms.RawValue.HasFlag(Channel.Value))
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError($"Bot needs channel permission for **{Channel.Value.ToString()}** to use this command.");
    }
}