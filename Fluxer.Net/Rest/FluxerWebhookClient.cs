using Serilog;

namespace Fluxer.Net;

public class FluxerWebhookClient
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

        _api = new ApiClient(this);
    }

    public FluxerConfig Config { get; }

    private ApiClient _api;

    public string WebhookUrl { get; }


}
