namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the command to be executed by the bot owner.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireOwnerAttribute : PreconditionAttribute
{
    /// <summary>
    /// Checks if the user is the bot owner.
    /// </summary>
    public override Task<PreconditionResult> CheckPermissionsAsync(
        CommandContext context,
        CommandInfo command,
        IServiceProvider? services)
    {
        // Get owner ID from service provider or environment
        var ownerIdString = Environment.GetEnvironmentVariable("BOT_OWNER_ID");

        if (string.IsNullOrWhiteSpace(ownerIdString) || !ulong.TryParse(ownerIdString, out var ownerId))
        {
            return Task.FromResult(PreconditionResult.FromError(
                "Bot owner ID not configured. Set BOT_OWNER_ID environment variable."));
        }

        if (context.User.Id == ownerId)
            return Task.FromResult(PreconditionResult.FromSuccess());

        return Task.FromResult(PreconditionResult.FromError(
            "This command can only be executed by the bot owner."));
    }
}
