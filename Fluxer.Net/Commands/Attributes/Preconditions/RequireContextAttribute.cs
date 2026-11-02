namespace Fluxer.Net.Commands;

/// <summary>
/// Requires the command to run in one of the given channel types.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireContextAttribute : PreconditionAttribute
{
    /// <summary>
    /// Creates a new <see cref="RequireContextAttribute"/> with the given channel types.
    /// </summary>
    /// <param name="contextTypes">The channel types the command may run in.</param>
    public RequireContextAttribute(ContextType contextTypes)
    {
        Contexts = contextTypes;
    }

    /// <summary>
    /// The channel types the command may run in.
    /// </summary>
    public ContextType Contexts { get; }

    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        bool isValid = false;

        if ((Contexts & ContextType.Community) != 0)
            isValid = context.Channel.GuildId.HasValue;
        if ((Contexts & ContextType.DM) != 0)
            isValid = isValid || context.Channel.Type == ChannelType.DM;
        if ((Contexts & ContextType.Group) != 0)
            isValid = isValid || context.Channel.Type == ChannelType.Group;

        if (isValid)
            return PreconditionResult.FromSuccess();

        return PreconditionResult.FromError($"You need to run this command in {Contexts} channel.");
    }
}

/// <summary>
/// The channel types a command may run in. Values can be combined.
/// </summary>
[Flags]
public enum ContextType
{
    /// <summary>
    /// A direct message with one user.
    /// </summary>
    DM = 0x01,

    /// <summary>
    /// A group direct message.
    /// </summary>
    Group = 0x02,

    /// <summary>
    /// A channel in a guild.
    /// </summary>
    Community = 0x04,

    /// <summary>
    /// The same value as <see cref="Community"/>.
    /// </summary>
    Guild = Community
}