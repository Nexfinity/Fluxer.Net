using Fluxer.Net.Commands.Builders;
using Fluxer.Net.Rest.Requests;

namespace Fluxer.Net.Commands;

/// <summary>
///     Provides a base class for a command module to inherit from with a <see cref="CommandContext"/>.
///  </summary>
public abstract class ModuleBase : IModuleBase
{
    /// <summary>
    ///     The underlying context of the command.
    /// </summary>
    public CommandContext? Context { get; private set; }


    /// <summary>
    /// Sends a message to the channel the command was executed in.
    /// </summary>
    protected virtual async Task<Message> ReplyAsync(string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        return await Context.Client.Rest.SendMessageAsync(Context.Channel.Id, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds, attachments);
    }

    /// <summary>
    ///     The method to execute before executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual void BeforeExecute(CommandInfo command)
    {
    }
    /// <summary>
    ///     The method to execute after executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual void AfterExecute(CommandInfo command)
    {
    }

    /// <summary>
    ///     The method to execute when building the module.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/> used to create the module.</param>
    /// <param name="builder">The builder used to build the module.</param>
    protected virtual void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
    {
    }

    //IModuleBase
    void IModuleBase.SetContext(CommandContext context)
    {
        Context = context;
    }
    void IModuleBase.BeforeExecute(CommandInfo command) => BeforeExecute(command);
    void IModuleBase.AfterExecute(CommandInfo command) => AfterExecute(command);
    void IModuleBase.OnModuleBuilding(CommandService commandService, ModuleBuilder builder) => OnModuleBuilding(commandService, builder);
}