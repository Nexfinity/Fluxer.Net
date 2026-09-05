namespace Fluxer.Net.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireContextAttribute : PreconditionAttribute
{
    public RequireContextAttribute(ContextType contextTypes)
    {
        Contexts = contextTypes;
    }

    /// <summary>
    /// Gets the context required to execute the command.
    /// </summary>
    public ContextType Contexts { get; }

    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        bool isValid = false;

        if ((Contexts & ContextType.Community) != 0)
            isValid = context.Channel.GuildId.HasValue;
        if ((Contexts & ContextType.DM) != 0)
            isValid = isValid || context.Channel.Type == ChannelType.Dm;
        if ((Contexts & ContextType.Group) != 0)
            isValid = isValid || context.Channel.Type == ChannelType.Group;

        if (isValid)
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError($"You need to run this command in {Contexts} channel.");
    }
}

[Flags]
public enum ContextType
{
    DM = 0x01,
    Group = 0x02,
    Community = 0x03
}