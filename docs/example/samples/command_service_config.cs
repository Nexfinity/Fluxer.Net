// Create the built-in command handler.
commands = new CommandService(new CommandServiceConfig
{
    // Configure the logger for the command service.
    Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger(),

    // Extra owner IDs when checking RequireOwner precondition.
    OwnerIds = new ulong[] { 12345 },
    
    // Owners can bypass permission preconditions.
    OwnerBypassPermissions = true
});