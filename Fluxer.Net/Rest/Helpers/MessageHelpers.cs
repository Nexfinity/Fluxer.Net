using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="Message"/> class. 
/// </summary>
public static class MessageHelpers
{
    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAsync(ulong, ulong)" />
    public static Task DeleteAsync(this Message message)
        => message.Client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);

    /// <inheritdoc cref="FluxerApiClient.EditMessageAsync(ulong, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, List{ulong}?, List{AttachmentRequest}?)" />
    public static Task<Message> ModifyAsync(this Message message, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMemeId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => message.Client.Rest.EditMessageAsync(message.ChannelId, message.Id, content, embeds, reference, allowedMentions, flags, nonce, favoriteMemeId, stickerIds, attachments);

    /// <inheritdoc cref="FluxerApiClient.AcknowledgeMessageAsync(ulong, ulong, MessageAckJson)" />
    public static Task AcknowledgeAsync(this Message message, MessageAckJson json)
        => message.Client.Rest.AcknowledgeMessageAsync(message.ChannelId, message.Id, json);

    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAttachmentAsync(ulong, ulong, ulong)" />
    public static Task DeleteAttachmentAsync(this Message message, ulong attachmentId)
        => message.Client.Rest.DeleteMessageAttachmentAsync(message.ChannelId, message.Id, attachmentId);

    /// <inheritdoc cref="FluxerApiClient.PinMessageAsync(ulong, ulong)" />
    public static Task PinAsync(this Message message)
        => message.Client.Rest.PinMessageAsync(message.ChannelId, message.Id);

    /// <inheritdoc cref="FluxerApiClient.UnpinMessageAsync(ulong, ulong)" />
    public static Task UnPinAsync(this Message message)
        => message.Client.Rest.UnpinMessageAsync(message.ChannelId, message.Id);

    /// <inheritdoc cref="FluxerApiClient.AddReactionAsync(ulong, ulong, string)" />
    public static Task AddReactionAsync(this Message message, string emoji)
        => message.Client.Rest.AddReactionAsync(message.ChannelId, message.Id, emoji);

    /// <inheritdoc cref="FluxerApiClient.GetReactionsForEmojiAsync(ulong, ulong, string)" />
    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Message message, string emoji)
        => message.Client.Rest.GetReactionsForEmojiAsync(message.ChannelId, message.Id, emoji);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsAsync(ulong, ulong)" />
    public static Task RemoveAllReactionsAsync(this Message message)
        => message.Client.Rest.RemoveAllReactionsAsync(message.ChannelId, message.Id);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsForEmojiAsync(ulong, ulong, string)" />
    public static Task RemoveAllReactionsForEmojiAsync(this Message message, string emoji)
        => message.Client.Rest.RemoveAllReactionsForEmojiAsync(message.ChannelId, message.Id, emoji);

    /// <inheritdoc cref="FluxerApiClient.RemoveOwnReactionAsync(ulong, ulong, string)" />
    public static Task RemoveOwnReactionAsync(this Message message, string emoji)
        => message.Client.Rest.RemoveOwnReactionAsync(message.ChannelId, message.Id, emoji);

    /// <inheritdoc cref="FluxerApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong)" />
    public static Task RemoveUserReactionAsync(this Message message, string emoji, ulong userId)
        => message.Client.Rest.RemoveUserReactionAsync(message.ChannelId, message.Id, emoji, userId);

    /// <inheritdoc cref="FluxerApiClient.SendMessageAsync(ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?)" />
    public static Task<Message> ReplyAsync(this Message message, string? content = null, List<EmbedRequest>? embeds = null,
        AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => message.Client.Rest.SendMessageAsync(message.ChannelId, content, embeds, new MessageReferenceRequest
        {
            MessageId = message.Id,
        }, allowedMentions, flags, nonce, favoriteMemeId, tts, stickerIds, attachments);

    /// <summary>
    /// Forward a message to another channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.SendMessages"/> in a guild channel.
    /// </remarks>
    public static Task<Message> ForwardAsync(this Message message, Channel channel, MessageFlag flags = MessageFlag.None, string? nonce = null)
        => message.Client.Rest.SendMessageAsync(channel.Id, null, null, new MessageReferenceRequest
        {
            Type = MessageReferenceType.Forward,
            MessageId = message.Id,
            ChannelId = message.ChannelId,
        }, null, flags, nonce);

    /// <summary>
    /// Hide embeds on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    public static Task<Message> SuppressEmbedsAsync(this Message message)
        => message.Client.Rest.EditMessageAsync(message.ChannelId, message.Id, flags: message.Flags |= MessageFlag.SuppressEmbeds);
}
