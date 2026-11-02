namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the bot to hold a guild or channel permission before the command executes.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// The guild permission the bot must hold, or null when a channel permission is checked.
    /// </summary>
    public GuildPermission? Guild { get; }

    /// <summary>
    /// The channel permission the bot must hold, or null when a guild permission is checked.
    /// </summary>
    public ChannelPermission? Channel { get; }

    /// <summary>
    /// Creates a new <see cref="RequireBotPermissionAttribute"/> that checks a guild permission.
    /// </summary>
    /// <param name="perm">The guild permission the bot must hold.</param>
    public RequireBotPermissionAttribute(GuildPermission perm)
    {
        Guild = perm;
    }

    /// <summary>
    /// Creates a new <see cref="RequireBotPermissionAttribute"/> that checks a channel permission.
    /// </summary>
    /// <param name="perm">The channel permission the bot must hold.</param>
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