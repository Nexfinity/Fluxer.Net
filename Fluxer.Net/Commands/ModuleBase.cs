using Fluxer.Net.Data.Models;

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
    /// Sends a message to the channel the command was executed in,
	/// and reply to the message that invoked the command.
    /// </summary>
    /// <param name="content">The message content.</param>
    protected async Task<Message> ReplyAsync(string content)
	{
		return await ReplyAsync(new Message
		{
			Content = content
		});
	}

	/// <summary>
	/// Sends a message to the channel the command was executed in,
	/// and reply to the message that invoked the command.
	/// </summary>
	/// <param name="message">The message to send.</param>
	protected async Task<Message> ReplyAsync(Message message)
	{
		// don't override ref
		if (message.Reference != null)
		{
			message.Reference = new MessageRef()
			{
				ChannelId = Context.Message.ChannelId,
				MessageId = Context.Message.Id
			};
        }
		return await Context.Client.SendMessage(Context.ChannelId, message);
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
