namespace Fluxer.Net.Commands;

/// <summary>
///     Requires the command to be invoked by the owner of the bot.
/// </summary>
/// <remarks>
///     This precondition will restrict the access of the command or module to the owner of the StoatSharp bot.
///     If the precondition fails to be met, an erroneous <see cref="PreconditionResult"/> will be returned with the
///     message "Command can only be run by the owner of the bot."
/// </remarks>
/// <example>
///     The following example restricts the command to a set of sensitive commands that only the owner of the bot
///     application should be able to access.
///     <code language="cs">
///     [RequireOwner]
///     [Group("admin")]
///     public class AdminModule : ModuleBase
///     {
///         [Command("exit")]
///         public async Task ExitAsync()
///         {
///             Environment.Exit(0);
///         }
///     }
///     </code>
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
