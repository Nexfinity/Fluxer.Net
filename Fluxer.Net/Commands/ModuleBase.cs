using Fluxer.Net.Rest;

namespace Fluxer.Net.Commands;

/// <summary>
/// The base class for a command module whose context is a <see cref="CommandContext"/>.
/// </summary>
public abstract class ModuleBase : ModuleBase<CommandContext> { }

/// <summary>
/// The base class for a command module whose context is of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The context type. Must implement <see cref="ICommandContext"/>.</typeparam>
public abstract class ModuleBase<T> : IModuleBase
    where T : class, ICommandContext
{
    #region ModuleBase
    /// <summary>
    /// The context of the command being executed.
    /// </summary>
    /// <seealso cref="ICommandContext" />
    /// <seealso cref="CommandContext" />
    public T Context { get; private set; }

    /// <summary>
    /// Sends a message to the channel the command was executed in.
    /// </summary>
    /// <param name="content">The text of the message.</param>
    /// <param name="embeds">The embeds to send with the message.</param>
    /// <param name="reference">The message to reply to.</param>
    /// <param name="allowedMentions">The mentions the message may trigger.</param>
    /// <param name="flags">The flags of the message.</param>
    /// <param name="nonce">The nonce of the message.</param>
    /// <param name="favoriteMediaId">The id of a favorite media item to attach.</param>
    /// <param name="tts">Whether the message is sent as text to speech.</param>
    /// <param name="stickerIds">The ids of the stickers to send.</param>
    /// <param name="attachments">The attachments to send with the message.</param>
    /// <returns>The message that was sent.</returns>
    protected virtual async Task<Message> ReplyAsync(string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        return await Context.Client.Rest.SendMessageAsync(Context.Channel.Id, content, embeds, reference, allowedMentions, flags, nonce, favoriteMediaId, tts, stickerIds, attachments);
    }

    /// <summary>
    /// Called asynchronously before the command runs. Does nothing by default.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual Task BeforeExecuteAsync(CommandInfo command) => Task.CompletedTask;
    /// <summary>
    /// Called before the command runs. Does nothing by default.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual void BeforeExecute(CommandInfo command)
    {
    }
    /// <summary>
    /// Called asynchronously after the command runs, even when it throws. Does nothing by default.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command that ran.</param>
    protected virtual Task AfterExecuteAsync(CommandInfo command) => Task.CompletedTask;
    /// <summary>
    /// Called after the command runs, even when it throws. Does nothing by default.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command that ran.</param>
    protected virtual void AfterExecute(CommandInfo command)
    {
    }

    /// <summary>
    /// Called when the module is being built. Does nothing by default.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/> used to create the module.</param>
    /// <param name="builder">The builder used to build the module.</param>
    protected virtual void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
    {
    }
    #endregion

    #region IModuleBase
    void IModuleBase.SetContext(ICommandContext context)
    {
        T newValue = context as T;
        Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}.");
    }
    Task IModuleBase.BeforeExecuteAsync(CommandInfo command) => BeforeExecuteAsync(command);
    void IModuleBase.BeforeExecute(CommandInfo command) => BeforeExecute(command);
    Task IModuleBase.AfterExecuteAsync(CommandInfo command) => AfterExecuteAsync(command);
    void IModuleBase.AfterExecute(CommandInfo command) => AfterExecute(command);
    void IModuleBase.OnModuleBuilding(CommandService commandService, ModuleBuilder builder) => OnModuleBuilding(commandService, builder);
    #endregion
}
