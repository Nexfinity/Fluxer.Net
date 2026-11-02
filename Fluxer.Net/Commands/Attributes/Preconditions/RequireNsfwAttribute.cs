namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the command to run in an NSFW channel.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireNsfwAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Channel.IsNsfw)
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError($"You need to use this command in a nsfw channel.");
    }
}