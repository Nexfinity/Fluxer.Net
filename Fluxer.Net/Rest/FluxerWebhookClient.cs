using Serilog;

namespace Fluxer.Net;

public class FluxerWebhookClient : BaseClient
{
    public FluxerWebhookClient(string webhookUrl, FluxerConfig config)
    {
        // TODO: Implement webhook validation
        WebhookUrl = webhookUrl;

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

    public string WebhookUrl { get; }


}
