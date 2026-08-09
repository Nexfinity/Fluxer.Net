namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireGroupOwnerAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Channel.Type != ChannelType.Group)
            return PreconditionResult.FromError("You need to run this command in a group channel.");

        if (context.User.Id == (context.Channel as GroupChannel).OwnerId)
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError("Command can only be run by the group owner.");
    }
}