using Fluxer.Net.Rest;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Webhook"/> class. 
/// </summary>
public static class WebhookHelpers
{
    /// <summary>
    /// The webhook's execute URL, or null when the webhook has no token.
    /// </summary>
    public static string? GetUrl(this Webhook webhook)
        => string.IsNullOrEmpty(webhook.Token) ? null : $"{webhook.Client.Config.RealApiBaseUrl}/webhooks/{webhook.Id}/{webhook.Token}";

    /// <inheritdoc cref="FluxerApiClient.DeleteWebhookWithTokenAsync(ulong, string, RequestOptions?)" />
    public static Task DeleteAsync(this Webhook webhook, RequestOptions? options = null)
        => webhook.Client.Rest.DeleteWebhookWithTokenAsync(webhook.Id, webhook.Token, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateWebhookWithTokenAsync{TRequest}(ulong, string, TRequest, RequestOptions?)" />
    public static Task<Webhook> ModifyAsync(this Webhook webhook, WebhookJson request, RequestOptions? options = null)
        => webhook.Client.Rest.UpdateWebhookWithTokenAsync(webhook.Id, webhook.Token, request, options);

    /// <inheritdoc cref="FluxerApiClient.ExecuteWebhookAsync(ulong, string, string?, List{EmbedRequest}?, string?, string?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task SendMessageAsync(this Webhook webhook, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null,
        RequestOptions? options = null)
        => webhook.Client.Rest.ExecuteWebhookAsync(webhook.Id, webhook.Token, content, embeds, username, avatarUrl, reference, allowedMentions, flags, nonce, favoriteMediaId, tts, stickerIds, attachments, options: options);

    /// <inheritdoc cref="FluxerApiClient.ExecuteWebhookWaitAsync(ulong, string, string?, List{EmbedRequest}?, string?, string?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> SendMessageWaitAsync(this Webhook webhook, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null,
        RequestOptions? options = null)
        => webhook.Client.Rest.ExecuteWebhookWaitAsync(webhook.Id, webhook.Token, content, embeds, username, avatarUrl, reference, allowedMentions, flags, nonce, favoriteMediaId, tts, stickerIds, attachments, options: options);

    /// <inheritdoc cref="FluxerApiClient.DeleteWebhookMessageAsync(ulong, string, ulong, RequestOptions?, ulong?)" />
    public static Task DeleteMessageAsync(this Webhook webhook, Message message, RequestOptions? options = null, ulong? threadId = null)
        => webhook.Client.Rest.DeleteWebhookMessageAsync(webhook.Id, webhook.Token, message.Id, options, threadId);

    /// <inheritdoc cref="FluxerApiClient.DeleteWebhookMessageAsync(ulong, string, ulong, RequestOptions?, ulong?)" />
    public static Task DeleteMessageAsync(this Webhook webhook, ulong messageId, RequestOptions? options = null, ulong? threadId = null)
        => webhook.Client.Rest.DeleteWebhookMessageAsync(webhook.Id, webhook.Token, messageId, options, threadId);

    /// <inheritdoc cref="FluxerApiClient.EditWebhookMessageAsync(ulong, string, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> EditMessageAsync(this Webhook webhook, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
        => webhook.Client.Rest.EditWebhookMessageAsync(webhook.Id, webhook.Token, messageId, content, embeds, reference, allowedMentions, flags, nonce, favoriteMediaId, stickerIds, attachments, options: options);

    /// <inheritdoc cref="FluxerApiClient.EditWebhookMessageAsync(ulong, string, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, List{ulong}?, List{AttachmentRequest}?, RequestOptions?)" />
    public static Task<Message> EditMessageAsync(this Webhook webhook, Message message, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
        => webhook.Client.Rest.EditWebhookMessageAsync(webhook.Id, webhook.Token, message.Id, content, embeds, reference, allowedMentions, flags, nonce, favoriteMediaId, stickerIds, attachments, options: options);
}
