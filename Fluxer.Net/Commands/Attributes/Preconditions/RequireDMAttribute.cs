namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireDMttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Channel.Type != ChannelType.Dm)
            return Task.FromResult(PreconditionResult.FromError("You need to run this command in a DM/Private channel."));

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}