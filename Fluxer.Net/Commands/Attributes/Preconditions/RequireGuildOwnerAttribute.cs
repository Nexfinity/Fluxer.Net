namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the command to run in a guild by the guild owner.
/// </summary>
/// <remarks>
/// When <see cref="CommandServiceConfig.OwnerBypassPermissions"/> is set, the bot owners pass this check.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireGuildOwnerAttribute : PreconditionAttribute
{
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

        if (context.User.Id == context.Guild.OwnerId)
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError("You need to run this command in a community.");
    }
}