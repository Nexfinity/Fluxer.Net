namespace Fluxer.Net.Commands.Attributes;

/// <summary>
/// Specifies where a command can be executed.
/// </summary>
public enum ContextType
{
    /// <summary>
    /// Command can only be executed in a guild.
    /// </summary>
    Guild = 1,

    /// <summary>
    /// Command can only be executed in a DM.
    /// </summary>
    DM = 2,

    /// <summary>
    /// Command can only be executed in a group DM.
    /// </summary>
    Group = 4
}

/// <summary>
/// Requires the command to be executed in a specific context.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireContextAttribute : PreconditionAttribute
{
    /// <summary>
    /// Gets the required contexts.
    /// </summary>
    public ContextType Contexts { get; }

    /// <summary>
    /// Creates a new context precondition.
    /// </summary>
    /// <param name="contexts">The allowed contexts.</param>
    public RequireContextAttribute(ContextType contexts)
    {
        Contexts = contexts;
    }

    /// <summary>
    /// Checks if the command is being used in the correct context.
    /// </summary>
    public override Task<PreconditionResult> CheckPermissionsAsync(
        CommandContext context,
        CommandInfo command,
        IServiceProvider? services)
    {
        bool isValid = false;

        if (Contexts.HasFlag(ContextType.Guild) && context.Guild != null)
            isValid = true;

        if (Contexts.HasFlag(ContextType.DM) && context.Guild == null)
            isValid = true;

        if (Contexts.HasFlag(ContextType.Group) && context.Channel.Type == ChannelType.GroupDm)
            isValid = true;

        if (isValid)
            return Task.FromResult(PreconditionResult.FromSuccess());

        return Task.FromResult(PreconditionResult.FromError(
            $"This command can only be used in: {Contexts}"));
    }
}
