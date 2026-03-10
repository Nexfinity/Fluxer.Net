namespace Fluxer.Net.Commands;

/// <summary>
/// Base class for command modules.
/// </summary>
public abstract class ModuleBase
{
    /// <summary>
    /// Gets the command context.
    /// </summary>
    public CommandContext Context { get; internal set; } = null!;

    /// <summary>
    /// Called before a command in this module is executed.
    /// </summary>
    protected internal virtual void BeforeExecute(CommandInfo command)
    {
    }

    /// <summary>
    /// Called after a command in this module is executed.
    /// </summary>
    protected internal virtual void AfterExecute(CommandInfo command)
    {
    }

    /// <summary>
    /// Sends a message to the channel the command was executed in.
    /// </summary>
    /// <param name="content">The message content.</param>
    protected async Task<MessageBaseResponse> ReplyAsync(string content)
    {
        return await Context.Rest.SendMessageAsync(Context.ChannelId, new Message { Content = content });
    }

    /// <summary>
    /// Sends a message to the channel the command was executed in.
    /// </summary>
    /// <param name="message">The message to send.</param>
    protected async Task<MessageBaseResponse> ReplyAsync(Message message)
    {
        return await Context.Rest.SendMessageAsync(Context.ChannelId, message);
    }
}

/// <summary>
/// Base class for command modules with a custom context type.
/// </summary>
/// <typeparam name="T">The context type.</typeparam>
public abstract class ModuleBase<T> : ModuleBase where T : CommandContext
{
    /// <summary>
    /// Gets the command context.
    /// </summary>
    public new T Context => (T)base.Context;
}
