using Fluxer.Net.Rest.Requests;

namespace Fluxer.Net;

public static class WebhookHelpers
{
    public static Task DeleteAsync(this Webhook webhook)
        => webhook.Client.Rest.DeleteWebhookWithTokenAsync(webhook.Id, webhook.Token);

    public static Task<Webhook> ModifyAsync(this Webhook webhook, WebhookJson request)
        => webhook.Client.Rest.UpdateWebhookWithTokenAsync(webhook.Id, webhook.Token, request);

    public static Task SendMessageAsync(this Webhook webhook, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null)
        => webhook.Client.Rest.ExecuteWebhookAsync(webhook.Id, webhook.Token, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);

    public static Task<Message> SendMessageWaitAsync(this Webhook webhook, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null)
        => webhook.Client.Rest.ExecuteWebhookWaitAsync(webhook.Id, webhook.Token, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);

    public static Task<Message> EditMessageAsync(this Webhook webhook, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null)
        => webhook.Client.Rest.EditWebhookMessageAsync(webhook.Id, webhook.Token, messageId, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);
}
