using Fluxer.Net.Rest.Requests;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="Webhook"/> class. 
/// </summary>
public static class WebhookHelpers
{
    /// <inheritdoc cref="ApiClient.DeleteWebhookWithTokenAsync(ulong, string)" />
    public static Task DeleteAsync(this Webhook webhook)
        => webhook.Client.Rest.DeleteWebhookWithTokenAsync(webhook.Id, webhook.Token);

    /// <inheritdoc cref="ApiClient.UpdateWebhookWithTokenAsync{TRequest}(ulong, string, TRequest)" />
    public static Task<Webhook> ModifyAsync(this Webhook webhook, WebhookJson request)
        => webhook.Client.Rest.UpdateWebhookWithTokenAsync(webhook.Id, webhook.Token, request);

    /// <inheritdoc cref="ApiClient.ExecuteWebhookAsync(ulong, string, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?)" />
    public static Task SendMessageAsync(this Webhook webhook, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null)
        => webhook.Client.Rest.ExecuteWebhookAsync(webhook.Id, webhook.Token, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);

    /// <inheritdoc cref="ApiClient.ExecuteWebhookAsync(ulong, string, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?)" />
    public static Task<Message> SendMessageWaitAsync(this Webhook webhook, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null)
        => webhook.Client.Rest.ExecuteWebhookWaitAsync(webhook.Id, webhook.Token, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);

    /// <inheritdoc cref="ApiClient.DeleteWebhookMessageAsync(ulong, string, ulong)" />
    public static Task DeleteMessageAsync(this Webhook webhook, Message message)
        => webhook.Client.Rest.EditWebhookMessageAsync(webhook.Id, webhook.Token, message.Id);

    /// <inheritdoc cref="ApiClient.DeleteWebhookMessageAsync(ulong, string, ulong)" />
    public static Task DeleteMessageAsync(this Webhook webhook, ulong messageId)
        => webhook.Client.Rest.EditWebhookMessageAsync(webhook.Id, webhook.Token, messageId);

    /// <inheritdoc cref="ApiClient.EditWebhookMessageAsync(ulong, string, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?)" />
    public static Task<Message> EditMessageAsync(this Webhook webhook, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null)
        => webhook.Client.Rest.EditWebhookMessageAsync(webhook.Id, webhook.Token, messageId, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);

    /// <inheritdoc cref="ApiClient.EditWebhookMessageAsync(ulong, string, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?)" />
    public static Task<Message> EditMessageAsync(this Webhook webhook, Message message, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null)
        => webhook.Client.Rest.EditWebhookMessageAsync(webhook.Id, webhook.Token, message.Id, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);
}
