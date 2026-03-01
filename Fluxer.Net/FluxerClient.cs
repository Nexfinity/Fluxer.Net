namespace Fluxer.Net;

public class FluxerClient
{
    public FluxerClient(string token, FluxerConfig? config = null)
    {
        Token = token;
        if (config == null)
            config = new FluxerConfig();

        Rest = new ApiClient(token, config);
        WebSocket = new GatewayClient(token, config);
    }

    public string Token { get; }

    public ApiClient Rest { get; }

    public GatewayClient WebSocket { get; }

    public Task StartAsync => WebSocket.ConnectAsync();
}
