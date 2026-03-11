using Serilog;

namespace Fluxer.Net;

public class FluxerWebhookClient : BaseClient
{
    public FluxerWebhookClient(string webhookUrl, FluxerConfig? config = null)
    {
        // TODO: Implement webhook validation
        string[] Split = webhookUrl.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (!webhookUrl.StartsWith("http") || Split.Length < 2)
            throw new ArgumentException("Invalid webhook url.");

        if (!ulong.TryParse(Split[Split.Length - 2], out ulong webhookId))
            throw new ArgumentException("Invalid webhook url.");

        Id = webhookId;
        Token = Split[Split.Length - 1];


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

    public FluxerConfig Config { get; }

    public ulong Id { get; }

    public string Token { get; }

    public Task<Webhook> GetAsync()
        => Rest.GetWebhookWithTokenAsync<Webhook>(Id, Token);

    public Task DeleteAsync()
        => Rest.DeleteWebhookWithTokenAsync(Id, Token);

    public Task<MessageBaseResponse> SendMessageAsync(Message message, StreamAttachment[]? attachments = null)
        => Rest.SendMessageAsync(Id, message, attachments);
}
