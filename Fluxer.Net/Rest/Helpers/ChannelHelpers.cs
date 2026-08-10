using Fluxer.Net.Rest;
using Fluxer.Net.Rest.Requests;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="Channel"/> class. 
/// </summary>
public static class ChannelHelpers
{
    /// <inheritdoc cref="ApiClient.TriggerTypingIndicatorAsync(ulong)" />
    public static Task TriggerTypingIndicatorAsync(this Channel channel)
        => channel.Client.Rest.TriggerTypingIndicatorAsync(channel.Id);

    /// <inheritdoc cref="ApiClient.GetChannelWebhooksAsync(ulong)" />
    public static Task<IEnumerable<Webhook>> GetWebhooksAsync(this Channel channel)
        => channel.Client.Rest.GetChannelWebhooksAsync(channel.Id);

    /// <inheritdoc cref="ApiClient.GetChannelInvitesAsync(ulong)" />
    public static Task<IEnumerable<Invite>> GetInvitesAsync(this Channel channel)
        => channel.Client.Rest.GetChannelInvitesAsync(channel.Id);

    /// <inheritdoc cref="ApiClient.CreateWebhookAsync(ulong, string, string?)" />
    public static Task<Webhook> CreateWebhookAsync(this Channel channel, string name, string? avatar = null)
        => channel.Client.Rest.CreateWebhookAsync(channel.Id, name, avatar);

    /// <inheritdoc cref="ApiClient.SendMessageAsync(ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?)" />
    public static Task<Message> SendMessageAsync(this Channel channel, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => channel.Client.Rest.SendMessageAsync(channel.Id, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds, attachments);

    /// <inheritdoc cref="ApiClient.SendMessageAsync(ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?)" />
    public static Task<Message> SendFilesAsync(this Channel channel, List<AttachmentRequest>? attachments, string? content = null)
        => channel.Client.Rest.SendMessageAsync(channel.Id, content, attachments: attachments);

    /// <inheritdoc cref="ApiClient.DeleteChannelAsync(ulong)" />
    public static Task DeleteAsync(this Channel channel)
        => channel.Client.Rest.DeleteChannelAsync(channel.Id);

    /// <inheritdoc cref="ApiClient.UpdateChannelAsync(ulong, ChannelJson)" />
    public static Task<Channel> ModifyAsync(this Channel channel, ChannelJson json)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, json);

    /// <inheritdoc cref="ApiClient.GetVoiceEligibilityAsync(ulong)" />
    public static Task<CallEligibility> GetVoiceEligibilityAsync(this Channel channel)
        => channel.Client.Rest.GetVoiceEligibilityAsync(channel.Id);

    /// <inheritdoc cref="ApiClient.UpdateVoiceRegionAsync(ulong, string?)" />
    public static Task UpdateVoiceRegionAsync(this Channel channel, string region)
        => channel.Client.Rest.UpdateVoiceRegionAsync(channel.Id, region);

    /// <inheritdoc cref="ApiClient.RingCallAsync(ulong, ulong[])" />
    public static Task RingCallAsync(this Channel channel, ulong[] recipients)
        => channel.Client.Rest.RingCallAsync(channel.Id, recipients);

    /// <inheritdoc cref="ApiClient.GetMessageAsync(ulong, ulong)" />
    public static Task<Message> GetMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.GetMessageAsync(channel.Id, messageId);

    /// <inheritdoc cref="ApiClient.GetMessagesAsync(ulong, int, ulong?, ulong?, ulong?, RestClientQueryParams?)" />
    public static Task<IEnumerable<Message>> GetMessagesAsync(this Channel channel, int limit = 100, ulong? beforeId = null, ulong? afterId = null, ulong? aroundId = null, RestClientQueryParams? queryParams = null)
        => channel.Client.Rest.GetMessagesAsync(channel.Id, limit, beforeId, afterId, aroundId, queryParams);

    /// <inheritdoc cref="ApiClient.DeleteMessageAsync(ulong, ulong)" />
    public static Task DeleteMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.DeleteMessageAsync(channel.Id, messageId);

    /// <inheritdoc cref="ApiClient.DeleteMessageAsync(ulong, ulong)" />
    public static Task DeleteMessageAsync(this Channel channel, Message message)
        => channel.Client.Rest.DeleteMessageAsync(channel.Id, message.Id);

    /// <inheritdoc cref="ApiClient.EditMessageAsync(ulong, ulong, UpdateMessageRequest)" />
    public static Task<Message> EditMessageAsync(this Channel channel, ulong messageId, UpdateMessageRequest json)
        => channel.Client.Rest.EditMessageAsync(channel.Id, messageId, json);

    /// <inheritdoc cref="ApiClient.EditMessageAsync(ulong, ulong, UpdateMessageRequest)" />
    public static Task<Message> EditMessageAsync(this Channel channel, Message message, UpdateMessageRequest json)
        => channel.Client.Rest.EditMessageAsync(channel.Id, message.Id, json);

    /// <inheritdoc cref="ApiClient.DeleteMessageAttachmentAsync(ulong, ulong, ulong)" />
    public static Task DeleteMessageAttachmentAsync(this Channel channel, ulong messageId, ulong attachmentId)
        => channel.Client.Rest.DeleteMessageAttachmentAsync(channel.Id, messageId, attachmentId);

    /// <inheritdoc cref="ApiClient.DeleteMessageAttachmentAsync(ulong, ulong, ulong)" />
    public static Task DeleteMessageAttachmentAsync(this Channel channel, Message message, ulong attachmentId)
        => channel.Client.Rest.DeleteMessageAttachmentAsync(channel.Id, message.Id, attachmentId);

    /// <inheritdoc cref="ApiClient.BulkDeleteMessagesAsync(ulong, BulkDeleteMessagesRequest)" />
    public static Task BulkDeleteMessagesAsync(this Channel channel, BulkDeleteMessagesRequest req)
        => channel.Client.Rest.BulkDeleteMessagesAsync(channel.Id, req);

    /// <inheritdoc cref="ApiClient.GetPinnedMessagesAsync(ulong, ChannelPinsQuery?)" />
    public static Task<ChannelPins> GetPinnedMessagesAsync(this Channel channel, ChannelPinsQuery? query = null)
        => channel.Client.Rest.GetPinnedMessagesAsync(channel.Id, query);

    /// <inheritdoc cref="ApiClient.PinMessageAsync(ulong, ulong)" />
    public static Task PinMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.PinMessageAsync(channel.Id, messageId);

    /// <inheritdoc cref="ApiClient.PinMessageAsync(ulong, ulong)" />
    public static Task PinMessageAsync(this Channel channel, Message message)
        => channel.Client.Rest.PinMessageAsync(channel.Id, message.Id);

    /// <inheritdoc cref="ApiClient.UnpinMessageAsync(ulong, ulong)" />
    public static Task UnPinMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.UnpinMessageAsync(channel.Id, messageId);

    /// <inheritdoc cref="ApiClient.UnpinMessageAsync(ulong, ulong)" />
    public static Task UnPinMessageAsync(this Channel channel, Message message)
        => channel.Client.Rest.UnpinMessageAsync(channel.Id, message.Id);

    /// <inheritdoc cref="ApiClient.AcknowledgeMessageAsync(ulong, ulong, MessageAckJson)" />
    public static Task AcknowledgeMessageAsync(this Channel channel, ulong messageId, MessageAckJson json)
        => channel.Client.Rest.AcknowledgeMessageAsync(channel.Id, messageId, json);

    /// <inheritdoc cref="ApiClient.AcknowledgeMessageAsync(ulong, ulong, MessageAckJson)" />
    public static Task AcknowledgeMessageAsync(this Channel channel, Message message, MessageAckJson json)
        => channel.Client.Rest.AcknowledgeMessageAsync(channel.Id, message.Id, json);

    /// <inheritdoc cref="ApiClient.ClearMessageAcknowledgementAsync(ulong)" />
    public static Task ClearMessageAcknowledgementAsync(this Channel channel)
        => channel.Client.Rest.ClearMessageAcknowledgementAsync(channel.Id);

    /// <inheritdoc cref="ApiClient.AddReactionAsync(ulong, ulong, string)" />
    public static Task AddReactionAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.AddReactionAsync(channel.Id, messageId, emoji);

    /// <inheritdoc cref="ApiClient.AddReactionAsync(ulong, ulong, string)" />
    public static Task AddReactionAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.AddReactionAsync(channel.Id, message.Id, emoji);

    /// <inheritdoc cref="ApiClient.GetReactionsForEmojiAsync(ulong, ulong, string)" />
    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.GetReactionsForEmojiAsync(channel.Id, messageId, emoji);

    /// <inheritdoc cref="ApiClient.GetReactionsForEmojiAsync(ulong, ulong, string)" />
    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.GetReactionsForEmojiAsync(channel.Id, message.Id, emoji);

    /// <inheritdoc cref="ApiClient.RemoveAllReactionsAsync(ulong, ulong)" />
    public static Task RemoveAllReactionsAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.RemoveAllReactionsAsync(channel.Id, messageId);

    /// <inheritdoc cref="ApiClient.RemoveAllReactionsAsync(ulong, ulong)" />
    public static Task RemoveAllReactionsAsync(this Channel channel, Message message)
        => channel.Client.Rest.RemoveAllReactionsAsync(channel.Id, message.Id);

    /// <inheritdoc cref="ApiClient.RemoveAllReactionsForEmojiAsync(ulong, ulong, string)" />
    public static Task RemoveAllReactionsForEmojiAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.RemoveAllReactionsForEmojiAsync(channel.Id, messageId, emoji);

    /// <inheritdoc cref="ApiClient.RemoveAllReactionsForEmojiAsync(ulong, ulong, string)" />
    public static Task RemoveAllReactionsForEmojiAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.RemoveAllReactionsForEmojiAsync(channel.Id, message.Id, emoji);

    /// <inheritdoc cref="ApiClient.RemoveOwnReactionAsync(ulong, ulong, string)" />
    public static Task RemoveOwnReactionAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.RemoveOwnReactionAsync(channel.Id, messageId, emoji);

    /// <inheritdoc cref="ApiClient.RemoveOwnReactionAsync(ulong, ulong, string)" />
    public static Task RemoveOwnReactionAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.RemoveOwnReactionAsync(channel.Id, message.Id, emoji);

    /// <inheritdoc cref="ApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong)" />
    public static Task RemoveUserReactionAsync(this Channel channel, ulong messageId, string emoji, ulong userId)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, messageId, emoji, userId);

    /// <inheritdoc cref="ApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong)" />
    public static Task RemoveUserReactionAsync(this Channel channel, Message message, string emoji, ulong userId)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, message.Id, emoji, userId);

    /// <inheritdoc cref="ApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong)" />
    public static Task RemoveUserReactionAsync(this Channel channel, Message message, string emoji, User user)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, message.Id, emoji, user.Id);
}
