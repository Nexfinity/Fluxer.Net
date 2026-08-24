FluxerClient _client = new FluxerClient(token);

ServiceCollection collection = new ServiceCollection()
        .AddSingleton(_client);

IServiceProvider services = collection.BuildServiceProvider();

await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);

await commands.ExecuteAsync(context, argPos, services);