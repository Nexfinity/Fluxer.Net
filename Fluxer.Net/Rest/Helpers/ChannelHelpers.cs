using Fluxer.Net.Rest;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Channel"/> class. 
/// </summary>
public static class ChannelHelpers
{
    /// <inheritdoc cref="FluxerApiClient.TriggerTypingIndicatorAsync(ulong, RequestOptions?)" />
    public static Task TriggerTypingIndicatorAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.TriggerTypingIndicatorAsync(channel.Id, options);

    /// <summary>
    /// Shows the typing indicator until the returned object is disposed, sending it again every 8 seconds.
    /// </summary>
    /// <example>
    /// <code>using (channel.EnterTypingState()) { await DoSlowWorkAsync(); }</code>
    /// </example>
    public static IDisposable EnterTypingState(this Channel channel)
        => new TypingState(channel.Client.Rest, channel.Id);

    private sealed class TypingState : IDisposable
    {
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private readonly Task _loop;

        public TypingState(FluxerApiClient rest, ulong channelId)
        {
            CancellationToken token = _cancel.Token;
            _loop = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await rest.TriggerTypingIndicatorAsync(channelId, new RequestOptions { CancelToken = token });
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(8), token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            });
        }

        public void Dispose()
        {
            _cancel.Cancel();
            _loop.ContinueWith(_ => _cancel.Dispose(), TaskScheduler.Default);
        }
    }

    /// <inheritdoc cref="FluxerApiClient.GetChannelWebhooksAsync(ulong, RequestOptions?)" />
    public static Task<IEnumerable<Webhook>> GetWebhooksAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.GetChannelWebhooksAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.GetChannelInvitesAsync(ulong, RequestOptions?)" />
    public static Task<IEnumerable<Invite>> GetInvitesAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.GetChannelInvitesAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.CreateWebhookAsync(ulong, string, string?, RequestOptions?)" />
    public static Task<Webhook> CreateWebhookAsync(this Channel channel, string name, string? avatar = null, RequestOptions? options = null)
        => channel.Client.Rest.CreateWebhookAsync(channel.Id, name, avatar, options);

    /// <inheritdoc cref="FluxerApiClient.SendMessageAsync(ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> SendMessageAsync(this Channel channel, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null
        , RequestOptions? options = null)
        => channel.Client.Rest.SendMessageAsync(channel.Id, content, embeds, reference, allowedMentions, flags, nonce, favoriteMediaId, tts, stickerIds, attachments, options: options);

    /// <inheritdoc cref="FluxerApiClient.SendMessageAsync(ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> SendFilesAsync(this Channel channel, List<AttachmentRequest>? attachments, string? content = null, RequestOptions? options = null)
        => channel.Client.Rest.SendMessageAsync(channel.Id, content, attachments: attachments, options: options);

    /// <inheritdoc cref="FluxerApiClient.DeleteChannelAsync(ulong, RequestOptions?)" />
    public static Task DeleteAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.DeleteChannelAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateChannelAsync(ulong, ChannelJson, RequestOptions?)" />
    [Obsolete("Every property of ChannelJson is sent, so this also resets the channel's age restriction and flags. Use the UpdateChannelRequest overload.")]
    public static Task<Channel> ModifyAsync(this Channel channel, ChannelJson json, RequestOptions? options = null)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, json, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateChannelAsync(ulong, UpdateChannelRequest, RequestOptions?)" />
    public static Task<Channel> ModifyAsync(this Channel channel, UpdateChannelRequest request, RequestOptions? options = null)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.GetVoiceEligibilityAsync(ulong, RequestOptions?)" />
    public static Task<CallEligibility> GetVoiceEligibilityAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.GetVoiceEligibilityAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateVoiceRegionAsync(ulong, string?, RequestOptions?)" />
    public static Task UpdateVoiceRegionAsync(this Channel channel, string region, RequestOptions? options = null)
        => channel.Client.Rest.UpdateVoiceRegionAsync(channel.Id, region, options);

    /// <inheritdoc cref="FluxerApiClient.RingCallAsync(ulong, ulong[], RequestOptions?)" />
    public static Task RingCallAsync(this Channel channel, ulong[] recipients, RequestOptions? options = null)
        => channel.Client.Rest.RingCallAsync(channel.Id, recipients, options);

    /// <inheritdoc cref="FluxerApiClient.GetMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task<Message?> GetMessageAsync(this Channel channel, ulong messageId, RequestOptions? options = null)
        => channel.Client.Rest.GetMessageAsync(channel.Id, messageId, options);

    /// <inheritdoc cref="FluxerApiClient.GetMessagesAsync(ulong, int, ulong?, ulong?, ulong?, RestClientQueryParams?, RequestOptions?)" />
    public static Task<IEnumerable<Message>> GetMessagesAsync(this Channel channel, int limit = 100, ulong? beforeId = null, ulong? afterId = null, ulong? aroundId = null, RestClientQueryParams? queryParams = null, RequestOptions? options = null)
        => channel.Client.Rest.GetMessagesAsync(channel.Id, limit, beforeId, afterId, aroundId, queryParams, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteMessageAsync(this Channel channel, ulong messageId, RequestOptions? options = null)
        => channel.Client.Rest.DeleteMessageAsync(channel.Id, messageId, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteMessageAsync(this Channel channel, Message message, RequestOptions? options = null)
        => channel.Client.Rest.DeleteMessageAsync(channel.Id, message.Id, options);

    /// <inheritdoc cref="FluxerApiClient.EditMessageAsync(ulong, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> EditMessageAsync(this Channel channel, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
        => channel.Client.Rest.EditMessageAsync(channel.Id, messageId, content, embeds, reference, allowedMentions, flags, nonce, favoriteMediaId, stickerIds, attachments, options);

    /// <inheritdoc cref="FluxerApiClient.EditMessageAsync(ulong, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> EditMessageAsync(this Channel channel, Message message, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
        => channel.Client.Rest.EditMessageAsync(channel.Id, message.Id, content, embeds, reference, allowedMentions, flags, nonce, favoriteMediaId, stickerIds, attachments, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAttachmentAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task DeleteMessageAttachmentAsync(this Channel channel, ulong messageId, ulong attachmentId, RequestOptions? options = null)
        => channel.Client.Rest.DeleteMessageAttachmentAsync(channel.Id, messageId, attachmentId, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteMessageAttachmentAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task DeleteMessageAttachmentAsync(this Channel channel, Message message, ulong attachmentId, RequestOptions? options = null)
        => channel.Client.Rest.DeleteMessageAttachmentAsync(channel.Id, message.Id, attachmentId, options);

    /// <inheritdoc cref="FluxerApiClient.BulkDeleteMessagesAsync(ulong, HashSet{ulong}, RequestOptions?)" />
    public static Task BulkDeleteMessagesAsync(this Channel channel, HashSet<ulong> messageIds, RequestOptions? options = null)
        => channel.Client.Rest.BulkDeleteMessagesAsync(channel.Id, messageIds, options);

    /// <inheritdoc cref="FluxerApiClient.GetPinnedMessagesAsync(ulong, ChannelPinsQuery?, RequestOptions?)" />
    public static Task<ChannelPins> GetPinnedMessagesAsync(this Channel channel, ChannelPinsQuery? query = null, RequestOptions? options = null)
        => channel.Client.Rest.GetPinnedMessagesAsync(channel.Id, query, options);

    /// <inheritdoc cref="FluxerApiClient.PinMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task PinMessageAsync(this Channel channel, ulong messageId, RequestOptions? options = null)
        => channel.Client.Rest.PinMessageAsync(channel.Id, messageId, options);

    /// <inheritdoc cref="FluxerApiClient.PinMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task PinMessageAsync(this Channel channel, Message message, RequestOptions? options = null)
        => channel.Client.Rest.PinMessageAsync(channel.Id, message.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UnpinMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task UnPinMessageAsync(this Channel channel, ulong messageId, RequestOptions? options = null)
        => channel.Client.Rest.UnpinMessageAsync(channel.Id, messageId, options);

    /// <inheritdoc cref="FluxerApiClient.UnpinMessageAsync(ulong, ulong, RequestOptions?)" />
    public static Task UnPinMessageAsync(this Channel channel, Message message, RequestOptions? options = null)
        => channel.Client.Rest.UnpinMessageAsync(channel.Id, message.Id, options);

    /// <summary>
    /// Gets the overwrite the channel stores for a role or member, or null when it has none.
    /// </summary>
    public static PermissionOverwrite? GetPermissionOverwrite(this Channel channel, ulong targetId)
        => channel.PermissionOverwrites?.FirstOrDefault(x => x.Id == targetId);

    /// <inheritdoc cref="FluxerApiClient.SetPermissionOverwriteAsync(ulong, ulong, PermissionOverwriteType, ChannelPermission, ChannelPermission, RequestOptions?)" />
    public static Task AddPermissionOverwriteAsync(this Channel channel, Role role, ChannelPermission allow,
        ChannelPermission deny, RequestOptions? options = null)
        => channel.Client.Rest.SetPermissionOverwriteAsync(channel.Id, role.Id, PermissionOverwriteType.Role, allow, deny, options);

    /// <inheritdoc cref="FluxerApiClient.SetPermissionOverwriteAsync(ulong, ulong, PermissionOverwriteType, ChannelPermission, ChannelPermission, RequestOptions?)" />
    public static Task AddPermissionOverwriteAsync(this Channel channel, GuildMember member, ChannelPermission allow,
        ChannelPermission deny, RequestOptions? options = null)
        => channel.Client.Rest.SetPermissionOverwriteAsync(channel.Id, member.Id, PermissionOverwriteType.Member, allow, deny, options);

    /// <inheritdoc cref="FluxerApiClient.SetPermissionOverwriteAsync(ulong, ulong, PermissionOverwriteType, ChannelPermission, ChannelPermission, RequestOptions?)" />
    public static Task AddPermissionOverwriteAsync(this Channel channel, ulong targetId, PermissionOverwriteType type,
        ChannelPermission allow, ChannelPermission deny, RequestOptions? options = null)
        => channel.Client.Rest.SetPermissionOverwriteAsync(channel.Id, targetId, type, allow, deny, options);

    /// <inheritdoc cref="FluxerApiClient.DeletePermissionOverwriteAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemovePermissionOverwriteAsync(this Channel channel, ulong targetId, RequestOptions? options = null)
        => channel.Client.Rest.DeletePermissionOverwriteAsync(channel.Id, targetId, options);

    /// <inheritdoc cref="FluxerApiClient.AcknowledgeMessageAsync(ulong, ulong, MessageAckJson, RequestOptions?)" />
    public static Task AcknowledgeMessageAsync(this Channel channel, ulong messageId, MessageAckJson json, RequestOptions? options = null)
        => channel.Client.Rest.AcknowledgeMessageAsync(channel.Id, messageId, json, options);

    /// <inheritdoc cref="FluxerApiClient.AcknowledgeMessageAsync(ulong, ulong, MessageAckJson, RequestOptions?)" />
    public static Task AcknowledgeMessageAsync(this Channel channel, Message message, MessageAckJson json, RequestOptions? options = null)
        => channel.Client.Rest.AcknowledgeMessageAsync(channel.Id, message.Id, json, options);

    /// <inheritdoc cref="FluxerApiClient.ClearMessageAcknowledgementAsync(ulong, RequestOptions?)" />
    public static Task ClearMessageAcknowledgementAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.ClearMessageAcknowledgementAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.AddReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task AddReactionAsync(this Channel channel, ulong messageId, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.AddReactionAsync(channel.Id, messageId, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.AddReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task AddReactionAsync(this Channel channel, Message message, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.AddReactionAsync(channel.Id, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.GetReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Channel channel, ulong messageId, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.GetReactionsForEmojiAsync(channel.Id, messageId, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.GetReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task<IEnumerable<User>> GetReactionsForEmojiAsync(this Channel channel, Message message, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.GetReactionsForEmojiAsync(channel.Id, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveAllReactionsAsync(this Channel channel, ulong messageId, RequestOptions? options = null)
        => channel.Client.Rest.RemoveAllReactionsAsync(channel.Id, messageId, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveAllReactionsAsync(this Channel channel, Message message, RequestOptions? options = null)
        => channel.Client.Rest.RemoveAllReactionsAsync(channel.Id, message.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveAllReactionsForEmojiAsync(this Channel channel, ulong messageId, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.RemoveAllReactionsForEmojiAsync(channel.Id, messageId, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveAllReactionsForEmojiAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveAllReactionsForEmojiAsync(this Channel channel, Message message, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.RemoveAllReactionsForEmojiAsync(channel.Id, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveOwnReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveOwnReactionAsync(this Channel channel, ulong messageId, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.RemoveOwnReactionAsync(channel.Id, messageId, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveOwnReactionAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task RemoveOwnReactionAsync(this Channel channel, Message message, string emoji, RequestOptions? options = null)
        => channel.Client.Rest.RemoveOwnReactionAsync(channel.Id, message.Id, emoji, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong, RequestOptions?)" />
    public static Task RemoveUserReactionAsync(this Channel channel, ulong messageId, string emoji, ulong userId, RequestOptions? options = null)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, messageId, emoji, userId, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong, RequestOptions?)" />
    public static Task RemoveUserReactionAsync(this Channel channel, Message message, string emoji, ulong userId, RequestOptions? options = null)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, message.Id, emoji, userId, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveUserReactionAsync(ulong, ulong, string, ulong, RequestOptions?)" />
    public static Task RemoveUserReactionAsync(this Channel channel, Message message, string emoji, User user, RequestOptions? options = null)
        => channel.Client.Rest.RemoveUserReactionAsync(channel.Id, message.Id, emoji, user.Id, options);

    /// <inheritdoc cref="FluxerApiClient.FollowChannelAsync(ulong, ulong, RequestOptions?)" />
    public static Task FollowAsync(this Channel channel, ulong targetChannelId, RequestOptions? options = null)
        => channel.Client.Rest.FollowChannelAsync(targetChannelId, channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.FollowChannelAsync(ulong, ulong, RequestOptions?)" />
    public static Task FollowAsync(this Channel channel, Channel targetChannel, RequestOptions? options = null)
        => channel.Client.Rest.FollowChannelAsync(targetChannel.Id, channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.CreateMessageThreadAsync(ulong, ulong, CreatePublicThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> CreateMessageThreadAsync(this Channel channel, ulong messageId, CreatePublicThreadRequest req, RequestOptions? options = null)
        => channel.Client.Rest.CreateMessageThreadAsync(channel.Id, messageId, req, options);

    /// <inheritdoc cref="FluxerApiClient.CreateMessageThreadAsync(ulong, ulong, CreatePublicThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> CreateMessageThreadAsync(this Channel channel, Message message, CreatePublicThreadRequest req, RequestOptions? options = null)
        => channel.Client.Rest.CreateMessageThreadAsync(channel.Id, message.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.CreatePublicThreadAsync(ulong, CreatePublicThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> CreatePublicThreadAsync(this Channel channel, CreatePublicThreadRequest req, RequestOptions? options = null)
        => channel.Client.Rest.CreatePublicThreadAsync(channel.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.CreatePrivateThreadAsync(ulong, CreatePrivateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> CreatePrivateThreadAsync(this Channel channel, CreatePrivateThreadRequest req, RequestOptions? options = null)
        => channel.Client.Rest.CreatePrivateThreadAsync(channel.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.GetThreadChannelAsync(ulong, RequestOptions?)" />
    public static Task<ThreadChannel?> GetThreadChannelAsync(this Channel channel, ulong threadId, RequestOptions? options = null)
        => channel.Client.Rest.GetThreadChannelAsync(threadId, options);

    /// <inheritdoc cref="FluxerApiClient.GetSlowmodeStateAsync(ulong, RequestOptions?)" />
    public static Task<SlowmodeStateJson> GetSlowmodeStateAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.GetSlowmodeStateAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.GetFollowerStatsAsync(ulong, RequestOptions?)" />
    public static Task<ChannelFollowerStatsJson> GetFollowerStatsAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.GetFollowerStatsAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.AcknowledgePinsAsync(ulong, RequestOptions?)" />
    public static Task AcknowledgePinsAsync(this Channel channel, RequestOptions? options = null)
        => channel.Client.Rest.AcknowledgePinsAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteOwnMessagesAsync(ulong, SudoVerificationRequest?, RequestOptions?)" />
    public static Task DeleteOwnMessagesAsync(this Channel channel, SudoVerificationRequest? verification = null, RequestOptions? options = null)
        => channel.Client.Rest.DeleteOwnMessagesAsync(channel.Id, verification, options);
}
