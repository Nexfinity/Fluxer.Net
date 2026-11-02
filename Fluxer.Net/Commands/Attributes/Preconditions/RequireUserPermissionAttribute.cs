namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the user to hold a guild or channel permission before the command executes.
/// </summary>
/// <remarks>
/// When <see cref="CommandServiceConfig.OwnerBypassPermissions"/> is set, the bot owners pass this check.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireUserPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// The guild permission the user must hold, or null when a channel permission is checked.
    /// </summary>
    public GuildPermission? Guild { get; }

    /// <summary>
    /// The channel permission the user must hold, or null when a guild permission is checked.
    /// </summary>
    public ChannelPermission? Channel { get; }

    /// <summary>
    /// Creates a new <see cref="RequireUserPermissionAttribute"/> that checks a guild permission.
    /// </summary>
    /// <param name="perm">The guild permission the user must hold.</param>
    public RequireUserPermissionAttribute(GuildPermission perm)
    {
        Guild = perm;
    }

    /// <summary>
    /// Creates a new <see cref="RequireUserPermissionAttribute"/> that checks a channel permission.
    /// </summary>
    /// <param name="perm">The channel permission the user must hold.</param>
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