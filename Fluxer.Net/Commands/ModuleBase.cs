using Fluxer.Net.Rest;
using Fluxer.Net.Rest.Requests;

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
    protected async Task<Message> ReplyAsync(string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        return await Context.Rest.SendMessageAsync(Context.ChannelId, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds, attachments);
    }

    /// <summary>
    /// Sends a message to the channel the command was executed in.
    /// </summary>
    protected async Task<Message> ReplyAsync(List<AttachmentRequest> attachments, string? content = null)
    {
        return await Context.Rest.SendMessageAsync(Context.ChannelId, content, attachments: attachments);
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
