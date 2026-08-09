namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireServerOwnerAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Guild == null)
            return PreconditionResult.FromError("You need to run this command in a server.");

        if (context.User.Id == context.Guild.OwnerId)
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError("Command can only be run by the server owner.");
    }
}