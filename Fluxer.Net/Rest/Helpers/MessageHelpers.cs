namespace Fluxer.Net;

public static class MessageHelpers
{
    public static Task DeleteAsync(this Message message)
        => message.Client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);

    public static Task<Message> ModifyAsync(this Message message, MessageUpdateRequest req)
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
}
