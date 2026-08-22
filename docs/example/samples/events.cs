private static FluxerClient _client;

public static async Task Main(string[] args)
{
    string token = "token_here";
    
    _client = new FluxerClient(token);

    await _client.StartAsync();

    _client.Gateway.MessageCreate += async (data) =>
    {
        Console.WriteLine($"{data.Author.Username}: {data.Content}");
    };

    await Task.Delay(-1);
}
