namespace Fluxer.Net;

public static class WebhookHelpers
{
    public static Task DeleteAsync(this Webhook webhook)
        => webhook.Client.Rest.DeleteWebhookWithTokenAsync(webhook.Id, webhook.Token);

    public static Task<Webhook> ModifyAsync(this Webhook webhook, WebhookJson request)
        => webhook.Client.Rest.UpdateWebhookWithTokenAsync(webhook.Id, webhook.Token, request);

    public static Task SendMessageAsync(this Webhook webhook, MessageJson request)
        => webhook.Client.Rest.ExecuteWebhookAsync(webhook.Id, webhook.Token, request);

    public static Task<Message> SendMessageWaitAsync(this Webhook webhook, MessageJson request)
        => webhook.Client.Rest.ExecuteWebhookWaitAsync(webhook.Id, webhook.Token, request);
}
