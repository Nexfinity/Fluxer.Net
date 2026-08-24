// Create the built-in command handler.
CommandService _commands = new CommandService();

// Load all modules in your project.
await _commands.AddModulesAsync(Assembly.GetExecutingAssembly());