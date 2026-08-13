using Fluxer.Net.Rest;
using Serilog;

namespace Fluxer.Net;

/// <summary>
/// Webhook client for Fluxer.
/// </summary>
public class FluxerWebhookClient : FluxerBaseClient
{
    /// <summary>
    /// Create a Webhook client with url to send webhook messages.
    /// </summary>
    /// <param name="webhookUrl"></param>
    /// <param name="config"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public FluxerWebhookClient(string webhookUrl, FluxerConfig? config = null)
    {
        // TODO: Implement webhook validation
        if (string.IsNullOrEmpty(webhookUrl))
            throw new ArgumentNullException("Missing webhook url.");

        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
#if !NETSTANDARD
        options |= StringSplitOptions.TrimEntries;
#endif

        string[] Split = webhookUrl.Split('/', options);
        if (!webhookUrl.StartsWith("http") || Split.Length < 2)
            throw new ArgumentException("Invalid webhook url.");

        if (!ulong.TryParse(Split[Split.Length - 2], out ulong webhookId))
            throw new ArgumentException("Invalid webhook url.");

        base.Id = webhookId;
        base.Token = Split[Split.Length - 1];


        // Load config
        if (config == null)
            config = new FluxerConfig();
        Config = config;

        // Load logger
        if (config.RestSerilog == null)
        {
            Config.RestSerilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console().CreateLogger();
        }

        Rest = new FluxerApiClient(this);
    }

    /// <summary>
    /// Webhook id.
    /// </summary>
    public new ulong Id => base.Id;

    /// <summary>
    /// Webhook token.
    /// </summary>
    public new string Token => base.Token;

    /// <inheritdoc cref="FluxerApiClient.GetWebhookWithTokenAsync(ulong, string)" />
    public Task<Webhook> GetAsync()
        => Rest.GetWebhookWithTokenAsync(Id, Token);

    /// <inheritdoc cref="FluxerApiClient.DeleteWebhookWithTokenAsync(ulong, string)" />
    public Task DeleteAsync()
        => Rest.DeleteWebhookWithTokenAsync(Id, Token);

    /// <inheritdoc cref="FluxerApiClient.ExecuteWebhookAsync(ulong, string, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?)" />
    public Task<Message> SendMessageAsync(string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => Rest.SendMessageAsync(Id, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds, attachments);

    /// <inheritdoc cref="FluxerApiClient.EditWebhookMessageAsync(ulong, string, ulong, string?, List{EmbedRequest}?, MessageReferenceRequest?, AllowedMentionsRequest?, MessageFlag, string?, ulong?, bool?, List{ulong}?)" />
    public Task<Message> EditMessageAsync(ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => Rest.EditWebhookMessageAsync(Id, Token, messageId, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);

    /// <inheritdoc cref="FluxerApiClient.DeleteWebhookMessageAsync(ulong, string, ulong)" />
    public Task DeleteMessageAsync(ulong messageId)
        => Rest.DeleteWebhookMessageAsync(Id, Token, messageId);
}
