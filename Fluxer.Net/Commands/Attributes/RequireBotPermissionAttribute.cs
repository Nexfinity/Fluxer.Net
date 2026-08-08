namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the bot to have specific permissions to execute the command.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// Gets the required guild permissions.
    /// </summary>
    public GuildPermission? GuildPermissions { get; }

    /// <summary>
    /// Gets the required guild permissions.
    /// </summary>
    public ChannelPermission? ChannelPermissions { get; }

    /// <summary>
    /// Creates a new bot permission precondition.
    /// </summary>
    /// <param name="permissions">The permissions required.</param>
    public RequireBotPermissionAttribute(GuildPermission permissions)
    {
        GuildPermissions = permissions;
    }

    /// <summary>
    /// Creates a new bot permission precondition.
    /// </summary>
    /// <param name="permissions">The permissions required.</param>
    public RequireBotPermissionAttribute(ChannelPermission permissions)
    {
        ChannelPermissions = permissions;
    }

    /// <summary>
    /// Checks if the bot has the required permissions.
    /// </summary>
    public override Task<PreconditionResult> CheckPermissionsAsync(
        CommandContext context,
        CommandInfo command,
        IServiceProvider? services)
    {
        // Must be in a guild context
        if (context.Guild == null)
            return Task.FromResult(PreconditionResult.FromSuccess());

        bool HasPermission = false;
        if (GuildPermissions.HasValue)
            HasPermission = context.Guild.CurrentMember.HasPermission(GuildPermissions.Value);

        if (ChannelPermissions.HasValue)
        {
            ChannelPermissions perms = context.Guild.CurrentMember.GetPermissions(context.Channel);
            HasPermission = perms.RawValue.HasFlag(ChannelPermissions);
        }

        if (!HasPermission)
            return Task.FromResult(PreconditionResult.FromError("Bot requires permissions."));

        // TODO: The Fluxer API doesn't currently expose permissions for guild members
        // This would require fetching guild roles and computing permissions based on role hierarchy
        // For now, this is a placeholder that always succeeds
        // Implementers should extend this to compute permissions from roles when needed

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
