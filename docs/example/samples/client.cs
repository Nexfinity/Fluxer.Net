private static FluxerClient _client;

public static async Task Main()
{
    // Put your bot token here.
    // For security you should use environment variables or config file later on.
    // string token = Environment.GetEnvironmentVariable("FLUXER_TOKEN");
    // string token = File.ReadAllText("token.txt");
    string token = "token_here";

    // You can configure logger settings using the second argument FluxerConfig.
    _client = new FluxerClient(token);

    // Recommended to use this for self-hosted fluxer instances.
    // This changes your FluxerConfig urls to use your instance.
    //await _client.LoginAsync("https://api.domain.com");
    
    // Use this for WebSocket connection and events otherwise use _client.Rest for rest-only use.
    await _client.StartAsync();

    // Block this task until the program is closed.
    await Task.Delay(-1);
}