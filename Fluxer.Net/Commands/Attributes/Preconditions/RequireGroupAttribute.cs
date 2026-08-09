namespace Fluxer.Net.Commands;

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