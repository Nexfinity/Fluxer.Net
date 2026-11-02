namespace Fluxer.Net.Commands;

/// <summary>
/// An exception that describes an exception thrown by a command, with the command and its context.
/// </summary>
/// <remarks>
/// The command service only logs this exception. When <see cref="CommandServiceConfig.ThrowOnError"/> is set, the
/// original exception is rethrown instead.
/// </remarks>
public class CommandException : Exception
{
    /// <summary>
    /// The command that threw the exception.
    /// </summary>
    public CommandInfo Command { get; }
    /// <summary>
    /// The context of the command that threw the exception.
    /// </summary>
    public ICommandContext Context { get; }

    /// <summary>
    /// Creates a new <see cref="CommandException"/> for a command that threw.
    /// </summary>
    /// <param name="command">The command that threw the exception.</param>
    /// <param name="context">The context of the command.</param>
    /// <param name="ex">The exception thrown by the command.</param>
    public CommandException(CommandInfo command, ICommandContext context, Exception ex)
        : base($"Error occurred executing {command.GetLogText(context)}.", ex)
    {
        Command = command;
        Context = context;
    }
}
