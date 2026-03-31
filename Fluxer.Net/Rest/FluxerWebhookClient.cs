using Fluxer.Net.Rest;
using Fluxer.Net.Rest.Requests;
using Serilog;

namespace Fluxer.Net;

public class FluxerWebhookClient : FluxerBaseClient
{
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

        Rest = new ApiClient(this);
    }

    public new ulong Id => base.Id;
    public new string Token => base.Token;

    public Task<Webhook> GetAsync()
        => Rest.GetWebhookWithTokenAsync(Id, Token);

    public Task DeleteAsync()
        => Rest.DeleteWebhookWithTokenAsync(Id, Token);

    public Task<Message> SendMessageAsync(string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => Rest.SendMessageAsync(Id, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds, attachments);

    public Task<Message> EditMessageAsync(ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
        => Rest.EditWebhookMessageAsync(Id, Token, messageId, content, embeds, reference, allowedMentions, flags, nonce, favoruteMemeId, tts, stickerIds);
}
