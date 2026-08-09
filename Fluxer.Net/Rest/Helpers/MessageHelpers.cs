using Fluxer.Net.Rest.Requests;

namespace Fluxer.Net;

public static class MessageHelpers
{
    public static Task DeleteAsync(this Message message)
        => message.Client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);

    public static Task<Message> ModifyAsync(this Message message, UpdateMessageRequest req)
        => message.Client.Rest.EditMessageAsync(message.ChannelId, message.Id, req);

    public static Task AcknowledgeAsync(this Message message, MessageAckJson json)
        => message.Client.Rest.AcknowledgeMessageAsync(message.ChannelId, message.Id, json);

    public static Task DeleteAttachmentAsync(this Message message, ulong attachmentId)
        => message.Client.Rest.DeleteMessageAttachmentAsync(message.ChannelId, message.Id, attachmentId);

    public static Task PinAsync(this Message message)
        => message.Client.Rest.PinMessageAsync(message.ChannelId, message.Id);

    public static Task UnPinAsync(this Message message)
        => message.Client.Rest.UnpinMessageAsync(message.ChannelId, message.Id);

    public static Task AddReactionAsync(this Message message, string emoji)
        => message.Client.Rest.AddReactionAsync(message.ChannelId, message.Id, emoji);

    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Message message, string emoji)
        => message.Client.Rest.GetReactionsForEmojiAsync(message.ChannelId, message.Id, emoji);

    public static Task RemoveAllReactionsAsync(this Message message)
        => message.Client.Rest.RemoveAllReactionsAsync(message.ChannelId, message.Id);

    public static Task RemoveAllReactionsForEmojiAsync(this Message message, string emoji)
        => message.Client.Rest.RemoveAllReactionsForEmojiAsync(message.ChannelId, message.Id, emoji);

    public static Task RemoveOwnReactionAsync(this Message message, string emoji)
        => message.Client.Rest.RemoveOwnReactionAsync(message.ChannelId, message.Id, emoji);

    public static Task RemoveUserReactionAsync(this Message message, string emoji, ulong userId)
        => message.Client.Rest.RemoveUserReactionAsync(message.ChannelId, message.Id, emoji, userId);

    public static Task<Message> ReplyAsync(this Message message, string? content = null, List<EmbedRequest>? embeds = null,
        AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => message.Client.Rest.SendMessageAsync(message.ChannelId, content, embeds, new MessageReferenceRequest
        {
            MessageId = message.Id,
        }, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds, attachments);

    public static Task<Message> ForwardAsync(this Message message, Channel channel, MessageFlag flags = MessageFlag.None, string? nonce = null)
        => message.Client.Rest.SendMessageAsync(channel.Id, null, null, new MessageReferenceRequest
        {
            Type = MessageReferenceType.Forward,
            MessageId = message.Id,
            ChannelId = message.ChannelId,
        }, null, flags, nonce);

    public static Task<Message> SuppressEmbedsAsync(this Message message, AllowedMentionsRequest? allowedMentions = null)
        => message.Client.Rest.EditMessageAsync(message.ChannelId, message.Id, new UpdateMessageRequest
        {
            AllowedMentions = allowedMentions,
            Flags = message.Flags |= MessageFlag.SuppressEmbeds,
        });
}
