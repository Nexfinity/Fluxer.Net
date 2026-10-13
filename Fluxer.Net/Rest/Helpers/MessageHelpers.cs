using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="Message"/> class. 
/// </summary>
public static class MessageHelpers
{
    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteAsync(this Message message, RequestOptions? options = null)
        => message.Client.Rest.DeleteMessageAsync(message.ChannelId, message.Id, options);

    /// <inheritdoc cref="FluxerApiClient.EditMessageAsync(ulong, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> ModifyAsync(this Message message, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
        => message.Client.Rest.EditMessageAsync(message.ChannelId, message.Id, content, embeds, reference, allowedMentions, flags, nonce, favoriteMediaId, stickerIds, attachments, options);

    /// <inheritdoc cref="FluxerApiClient.AcknowledgeMessageAsync(ulong, ulong, MessageAckJson, RequestOptions?)" />
    public static Task AcknowledgeAsync(this Message message, MessageAckJson json, RequestOptions? options = null)
        => message.Client.Rest.AcknowledgeMessageAsync(message.ChannelId, message.Id, json, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAttachmentAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task DeleteAttachmentAsync(this Message message, ulong attachmentId, RequestOptions? options = null)
        => message.Client.Rest.DeleteMessageAttachmentAsync(message.ChannelId, message.Id, attachmentId, options);

    /// <inheritdoc cref="FluxerApiClient.PinMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task PinAsync(this Message message, RequestOptions? options = null)
        => message.Client.Rest.PinMessageAsync(message.ChannelId, message.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UnpinMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task UnPinAsync(this Message message, RequestOptions? options = null)
        => message.Client.Rest.UnpinMessageAsync(message.ChannelId, message.Id, options);

    /// <inheritdoc cref="FluxerApiClient.AddReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task AddReactionAsync(this Message message, string emoji, RequestOptions? options = null)
        => message.Client.Rest.AddReactionAsync(message.ChannelId, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.GetReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Message message, string emoji, RequestOptions? options = null)
        => message.Client.Rest.GetReactionsForEmojiAsync(message.ChannelId, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveAllReactionsAsync(this Message message, RequestOptions? options = null)
        => message.Client.Rest.RemoveAllReactionsAsync(message.ChannelId, message.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveAllReactionsForEmojiAsync(this Message message, string emoji, RequestOptions? options = null)
        => message.Client.Rest.RemoveAllReactionsForEmojiAsync(message.ChannelId, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveOwnReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveOwnReactionAsync(this Message message, string emoji, RequestOptions? options = null)
        => message.Client.Rest.RemoveOwnReactionAsync(message.ChannelId, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong, RequestOptions?)" />
    public static Task RemoveUserReactionAsync(this Message message, string emoji, ulong userId, RequestOptions? options = null)
        => message.Client.Rest.RemoveUserReactionAsync(message.ChannelId, message.Id, emoji, userId, options);


    /// <inheritdoc cref="FluxerApiClient.AddReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task AddReactionAsync(this Message message, Emoji emoji, RequestOptions? options = null)
        => message.Client.Rest.AddReactionAsync(message.ChannelId, message.Id, emoji.ToRequest(), options);

    /// <inheritdoc cref="FluxerApiClient.GetReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Message message, Emoji emoji, RequestOptions? options = null)
        => message.Client.Rest.GetReactionsForEmojiAsync(message.ChannelId, message.Id, emoji.ToRequest(), options);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveAllReactionsForEmojiAsync(this Message message, Emoji emoji, RequestOptions? options = null)
        => message.Client.Rest.RemoveAllReactionsForEmojiAsync(message.ChannelId, message.Id, emoji.ToRequest(), options);

    /// <inheritdoc cref="FluxerApiClient.RemoveOwnReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveOwnReactionAsync(this Message message, Emoji emoji, RequestOptions? options = null)
        => message.Client.Rest.RemoveOwnReactionAsync(message.ChannelId, message.Id, emoji.ToRequest(), options);

    /// <inheritdoc cref="FluxerApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong, RequestOptions?)" />
    public static Task RemoveUserReactionAsync(this Message message, Emoji emoji, User user, RequestOptions? options = null)
        => message.Client.Rest.RemoveUserReactionAsync(message.ChannelId, message.Id, emoji.ToRequest(), user.Id, options);


    /// <inheritdoc cref="FluxerApiClient.SendMessageAsync(ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> ReplyAsync(this Message message, string? content = null, List<EmbedRequest>? embeds = null,
        AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
        => message.Client.Rest.SendMessageAsync(message.ChannelId, content, embeds, new MessageReferenceRequest
        {
            MessageId = message.Id,
        }, allowedMentions, flags, nonce, favoriteMediaId, tts, stickerIds, attachments, options);

    /// <summary>
    /// Forward a message to another channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.SendMessages"/> in a guild channel.
    /// </remarks>
    public static Task<Message> ForwardAsync(this Message message, Channel channel, MessageFlag flags = MessageFlag.None, string? nonce = null, RequestOptions? options = null)
        => message.Client.Rest.SendMessageAsync(channel.Id, null, null, new MessageReferenceRequest
        {
            Type = MessageReferenceType.Forward,
            MessageId = message.Id,
            ChannelId = message.ChannelId,
        }, null, flags, nonce, options: options);

    /// <inheritdoc cref="FluxerApiClient.CrosspostMessageAsync(ulong, ulong, RequestOptions?)"/>
    public static Task CrosspostAsync(this Message message, RequestOptions? options = null)
        => message.Client.Rest.CrosspostMessageAsync(message.ChannelId, message.Id, options);

    /// <summary>
    /// Hide embeds on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    public static Task<Message> SuppressEmbedsAsync(this Message message, RequestOptions? options = null)
        => message.Client.Rest.EditMessageAsync(message.ChannelId, message.Id, flags: message.Flags | MessageFlag.SuppressEmbeds, options: options);

    /// <inheritdoc cref="FluxerApiClient.CreateMessageThreadAsync(ulong, ulong, CreatePublicThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> CreateThreadAsync(this Message message, CreatePublicThreadRequest req, RequestOptions? options = null)
        => message.Client.Rest.CreateMessageThreadAsync(message.ChannelId, message.Id, req, options: options);
}
