using Fluxer.Net.Rest;
using Fluxer.Net.Rest.Requests;

namespace Fluxer.Net;

public static class ChannelHelpers
{
    public static Task<Message> SendMessageAsync(this Channel channel, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => channel.Client.Rest.SendMessageAsync(channel.Id, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds, attachments);

    public static Task<Message> SendFilesAsync(this Channel channel, List<AttachmentRequest>? attachments, string? content = null)
        => channel.Client.Rest.SendMessageAsync(channel.Id, content, attachments: attachments);

    public static Task DeleteAsync(this Channel channel)
        => channel.Client.Rest.DeleteChannelAsync(channel.Id);

    public static Task<Channel> ModifyAsync(this Channel channel, ChannelJson json)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, json);

    public static Task<CallEligibility> GetVoiceEligibilityAsync(this Channel channel)
        => channel.Client.Rest.GetVoiceEligibilityAsync(channel.Id);

    public static Task UpdateVoiceRegionAsync(this Channel channel, string region)
        => channel.Client.Rest.UpdateVoiceRegionAsync(channel.Id, region);

    public static Task RingCallAsync(this Channel channel, ulong[] recipients)
        => channel.Client.Rest.RingCallAsync(channel.Id, recipients);

    public static Task<Message> GetMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.GetMessageAsync(channel.Id, messageId);

    public static Task<IEnumerable<Message>> GetMessagesAsync(this Channel channel, int limit = 100, ulong? beforeId = null, ulong? afterId = null, ulong? aroundId = null, RestClientQueryParams? queryParams = null)
        => channel.Client.Rest.GetMessagesAsync(channel.Id, limit, beforeId, afterId, aroundId, queryParams);

    public static Task DeleteMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.DeleteMessageAsync(channel.Id, messageId);

    public static Task DeleteMessageAsync(this Channel channel, Message message)
        => channel.Client.Rest.DeleteMessageAsync(channel.Id, message.Id);

    public static Task<Message> EditMessageAsync(this Channel channel, ulong messageId, UpdateMessageRequest json)
        => channel.Client.Rest.EditMessageAsync(channel.Id, messageId, json);

    public static Task<Message> EditMessageAsync(this Channel channel, Message message, UpdateMessageRequest json)
        => channel.Client.Rest.EditMessageAsync(channel.Id, message.Id, json);

    public static Task DeleteMessageAttachmentAsync(this Channel channel, ulong messageId, ulong attachmentId)
        => channel.Client.Rest.DeleteMessageAttachmentAsync(channel.Id, messageId, attachmentId);

    public static Task DeleteMessageAttachmentAsync(this Channel channel, Message message, ulong attachmentId)
        => channel.Client.Rest.DeleteMessageAttachmentAsync(channel.Id, message.Id, attachmentId);

    public static Task BulkDeleteMessagesAsync(this Channel channel, BulkDeleteMessagesRequest req)
        => channel.Client.Rest.BulkDeleteMessagesAsync(channel.Id, req);

    public static Task<ChannelPins> GetPinnedMessagesAsync(this Channel channel, ChannelPinsQuery? query = null)
        => channel.Client.Rest.GetPinnedMessagesAsync(channel.Id, query);

    public static Task PinMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.PinMessageAsync(channel.Id, messageId);

    public static Task PinMessageAsync(this Channel channel, Message message)
        => channel.Client.Rest.PinMessageAsync(channel.Id, message.Id);

    public static Task UnPinMessageAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.UnpinMessageAsync(channel.Id, messageId);

    public static Task UnPinMessageAsync(this Channel channel, Message message)
        => channel.Client.Rest.UnpinMessageAsync(channel.Id, message.Id);

    public static Task AcknowledgeMessageAsync(this Channel channel, ulong messageId, MessageAckJson json)
        => channel.Client.Rest.AcknowledgeMessageAsync(channel.Id, messageId, json);

    public static Task AcknowledgeMessageAsync(this Channel channel, Message message, MessageAckJson json)
        => channel.Client.Rest.AcknowledgeMessageAsync(channel.Id, message.Id, json);

    public static Task ClearMessageAcknowledgementAsync(this Channel channel)
        => channel.Client.Rest.ClearMessageAcknowledgementAsync(channel.Id);

    public static Task AddReactionAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.AddReactionAsync(channel.Id, messageId, emoji);

    public static Task AddReactionAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.AddReactionAsync(channel.Id, message.Id, emoji);

    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.GetReactionsForEmojiAsync(channel.Id, messageId, emoji);

    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.GetReactionsForEmojiAsync(channel.Id, message.Id, emoji);

    public static Task RemoveAllReactionsAsync(this Channel channel, ulong messageId)
        => channel.Client.Rest.RemoveAllReactionsAsync(channel.Id, messageId);

    public static Task RemoveAllReactionsAsync(this Channel channel, Message message)
        => channel.Client.Rest.RemoveAllReactionsAsync(channel.Id, message.Id);

    public static Task RemoveAllReactionsForEmojiAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.RemoveAllReactionsForEmojiAsync(channel.Id, messageId, emoji);

    public static Task RemoveAllReactionsForEmojiAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.RemoveAllReactionsForEmojiAsync(channel.Id, message.Id, emoji);

    public static Task RemoveOwnReactionAsync(this Channel channel, ulong messageId, string emoji)
        => channel.Client.Rest.RemoveOwnReactionAsync(channel.Id, messageId, emoji);

    public static Task RemoveOwnReactionAsync(this Channel channel, Message message, string emoji)
        => channel.Client.Rest.RemoveOwnReactionAsync(channel.Id, message.Id, emoji);

    public static Task RemoveUserReactionAsync(this Channel channel, ulong messageId, string emoji, ulong userId)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, messageId, emoji, userId);

    public static Task RemoveUserReactionAsync(this Channel channel, Message message, string emoji, ulong userId)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, message.Id, emoji, userId);

    public static Task RemoveUserReactionAsync(this Channel channel, Message message, string emoji, User user)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, message.Id, emoji, user.Id);
}
