namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the command to run in a group direct message channel.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireGroupAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Channel.Type != ChannelType.Group)
            return PreconditionResult.FromError("You need to run this command in a group channel.");

        return PreconditionResult.FromSuccess();
    }
}