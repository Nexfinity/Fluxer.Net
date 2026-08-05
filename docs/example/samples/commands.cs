private static FluxerClient _client;
private static CommandService _commands;

public static async Task Main()
{
    string token = "token_here";
    _client = new FluxerClient(token);

    // This is required to recieve text based commands.
    await _client.StartAsync();

    // Create the built-in command handler.
    _commands = new CommandService();

    // Load all modules in your project.
    await _commands.AddModulesAsync(Assembly.GetExecutingAssembly());

    string prefix = "!";

    // Listen for messages with the starting prefix and command !test
    Client.Gateway.MessageCreate += async (data) =>
    {
        // Ignore messages without an author (system messages, webhooks) and exclude bots.
        if (data.Author == null || data.Author.IsBot)
            return;

        // Check if message starts with the prefix
        int argPos = 0;
        if (data.Content?.StartsWith(Prefix) == true)
            argPos = Prefix.Length;

        if (argPos == 0)
            return;
        
        // Create a command context
        CommandContext context = new CommandContext(Client, data);

        // Execute the command
        IResult result = await commands.ExecuteAsync(context, argPos);

        if (result.IsSuccess)
            return;

        // Optionally send error message to user
        if (result.ErrorType == CommandError.BadArgCount ||
            result.ErrorType == CommandError.ParseFailed ||
            result.ErrorType == CommandError.UnmetPrecondition ||
            result.ErrorType == CommandError.Exception)
        {
            try
            {
                await context.Channel.SendMessageAsync($"❌ Error: {result.Error}");
            }
            catch { }
        }
    };


    // Block this task until the program is closed.
    await Task.Delay(-1);
}