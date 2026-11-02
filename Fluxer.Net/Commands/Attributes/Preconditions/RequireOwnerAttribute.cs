namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the command to be invoked by the owner of the bot.
/// </summary>
/// <remarks>
/// The owner is a user listed in <see cref="CommandServiceConfig.OwnerIDs"/> or the bot owner reported by the gateway.
/// If the precondition is not met, the returned <see cref="PreconditionResult"/> has the message
/// "Command can only be run by the owner of the bot."
/// </remarks>
/// <example>
/// The following example restricts the commands in the module to the owner of the bot.
/// <code language="cs">
/// [RequireOwner]
/// [Group("admin")]
/// public class AdminModule : ModuleBase
/// {
///     [Command("exit")]
///     public async Task ExitAsync()
///     {
///         Environment.Exit(0);
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireOwnerAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.CommandService._ownerIds?.Any(x => x == context.User.Id) == true)
            return PreconditionResult.FromSuccess();

        ulong? OwnerId = await context.Gateway.GetOwnerIdAsync();
        if (OwnerId.HasValue && OwnerId.Value == context.User.Id)
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError("Command can only be run by the owner of the bot.");
    }
}
