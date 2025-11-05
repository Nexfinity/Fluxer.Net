// ============================================================================
// Fluxer.Net Example Project - Getting Started Tutorial
// ============================================================================
// This example demonstrates the core concepts of building a Fluxer bot using
// the Fluxer.Net library. You'll learn how to:
//   1. Configure logging for debugging and monitoring
//   2. Set up both the Gateway (real-time events) and API (REST operations)
//   3. Handle gateway events (like messages)
//   4. Make API calls (like sending messages)
//   5. Implement basic command handling
//
// Prerequisites:
//   - A Fluxer account and bot token (add it to config.yml)
//   - .NET 7.0 or higher
//   - Basic understanding of async/await in C#
// ============================================================================

using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using Fluxer.Net;
using Fluxer.Net.Data.Enums;
using Fluxer.Net.Example;
using Fluxer.Net.Gateway.Data;

// ============================================================================
// STEP 1: Configure Logging
// ============================================================================
// Serilog provides structured logging for the Fluxer.Net library. This helps
// you debug issues and monitor your bot's activity. Logs are written to both
// the console (for development) and a file (for production debugging).

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()  // Log everything (Verbose, Debug, Info, Warning, Error, Fatal)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)  // Pretty console output with colors
                .WriteTo.File($"output-{DateTime.Now:yyyy-MM-dd:hh-mm-ss}.log",
                              rollingInterval: RollingInterval.Infinite,
                              rollOnFileSizeLimit: true)  // Time-stamped log files
                .CreateLogger();

// ============================================================================
// STEP 2: Load Your Bot Token
// ============================================================================
// The token authenticates your bot with the Fluxer API. NEVER commit your
// token to version control! Store it in config.yml (which is .gitignored).
//
// To get a token:
//   1. Create a bot in the Fluxer developer portal
//   2. Copy the bot token
//   3. Paste it into config.yml as "Token: flx_your_token_here"

var config = ConfigExtension.LoadConfig();
if (config == null)
{
    Log.Error("YAML file not found. Please create a config.yml file with your bot token.");
    Log.Error("Example format:\n  Token: flx_your_token_here");
    return;
}

Log.Debug("Config file loaded successfully.");

// ============================================================================
// STEP 3: Initialize the Gateway Client (Real-Time Events)
// ============================================================================
// The GatewayClient connects to Fluxer's WebSocket gateway to receive real-time
// events like messages, reactions, member joins, etc. This is the "listening"
// part of your bot that responds to what happens on Fluxer.
//
// Key Configuration Options:
//   - ReconnectAttemptDelay: Seconds to wait between reconnection attempts
//   - Serilog: Logger instance for gateway events
//   - IgnoredGatewayEvents: Filter out events you don't need (reduces processing)
//   - Presence: Your bot's initial status (Online, Idle, DND, Invisible)

var gateway = new GatewayClient(config["Token"], new()
{
    ReconnectAttemptDelay = 2,  // Reconnect quickly if connection drops
    Serilog = Log.Logger as Logger,  // Use our configured logger

    // Ignore high-volume events we don't need to reduce CPU/memory usage
    // Common events to ignore: PRESENCE_UPDATE, TYPING_START, VOICE_STATE_UPDATE
    IgnoredGatewayEvents = new()
    {
        "PRESENCE_UPDATE"  // We don't need to track when users go online/offline
    },

    // Set your bot's status. Options: Online, Idle, DND, Invisible
    Presence = new PresenceUpdateGatewayData(Status.Online)
});

// ============================================================================
// STEP 4: Initialize the API Client (REST Operations)
// ============================================================================
// The ApiClient handles REST API requests for creating, reading, updating, and
// deleting resources (messages, channels, guilds, users, etc.). This is the
// "action" part of your bot that makes changes on Fluxer.
//
// Key Features:
//   - Automatic rate limiting (enabled by default via sliding window algorithm)
//   - Token-based authentication
//   - Full coverage of 150+ Fluxer API endpoints
//   - Shared logging configuration with the gateway

var api = new ApiClient(config[key: "Token"], new()
{
    Serilog = Log.Logger as Logger,  // Use our configured logger
    EnableRateLimiting = true  // Prevent hitting rate limits (default: true)
});

// ============================================================================
// STEP 5: Handle Command-Line Arguments
// ============================================================================
// This example supports a --revoke flag to log out the bot and invalidate
// the current token. Useful for testing or emergency shutdowns.
//
// Usage: dotnet run --revoke

if (args.Length > 0 && args[0] == "--revoke")
{
    Log.Information("Revoking token and logging out...");
    await api.Logout();
    Log.Information("Token revoked successfully. The bot is now logged out.");
    return;
}

// ============================================================================
// STEP 6: Example API Call - Update Bot Nickname
// ============================================================================
// This demonstrates a simple API call to update the bot's nickname in a guild.
// Replace the guild ID with your own guild/community ID.
//
// To find your guild ID:
//   1. Enable developer mode in Fluxer settings
//   2. Right-click your guild/community
//   3. Click "Copy ID"

// NOTE: Replace this guild ID with your own!
// await api.UpdateCurrentMember(1431484523333775609, new()
// {
//     Nickname = "Fluxer.Net Example Bot"
// });

// ============================================================================
// STEP 7: Subscribe to Gateway Events
// ============================================================================
// The gateway uses an event-driven architecture. You subscribe to events by
// attaching handlers to the GatewayClient. Here we demonstrate basic command
// handling by listening for MESSAGE_CREATE events.
//
// Available Events (just a few examples):
//   - MessageCreate: New message posted
//   - MessageUpdate: Message edited
//   - MessageDelete: Message deleted
//   - GuildCreate: Bot added to a guild
//   - GuildMemberAdd: User joined a guild
//   - MessageReactionAdd: Reaction added to a message
//   ... and many more! See GatewayClient.cs for the full list.

gateway.MessageCreate += async messageData =>
{
    try
    {
        // Ignore messages from webhooks or system messages (they don't have an Author)
        if (messageData.Author == null)
            return;

        // Log every message for debugging (optional - can be noisy!)
        Log.Debug("Message received in channel {ChannelId} from {Username}: {Content}",
            messageData.ChannelId, messageData.Author.Username, messageData.Content);

        // ========================================================================
        // Example Command: /ping
        // ========================================================================
        // Simple command that responds with "pong" when a user types "/ping"

        if (messageData.Content == "/ping")
        {
            Log.Information("Ping command received from user {Username} ({UserId})",
                messageData.Author.Username, messageData.Author.Id);

            // Send a response message to the same channel
            await api.SendMessage(messageData.ChannelId, new()
            {
                Content = "pong ;P"
            });
        }

        // ========================================================================
        // Example Command: /hello
        // ========================================================================
        // Demonstrates mentioning the user who sent the command

        else if (messageData.Content == "/hello")
        {
            await api.SendMessage(messageData.ChannelId, new()
            {
                Content = $"Hello, <@{messageData.Author.Id}>! 👋"
            });
        }

        // ========================================================================
        // Example Command: /info
        // ========================================================================
        // Demonstrates sending a formatted message with multiple lines

        else if (messageData.Content == "/info")
        {
            await api.SendMessage(messageData.ChannelId, new()
            {
                Content = $"**Fluxer.Net Example Bot**\n" +
                          $"Version: 0.4.0\n" +
                          $"Framework: .NET 7.0\n" +
                          $"Library: Fluxer.Net\n\n" +
                          $"Available Commands:\n" +
                          $"• `/ping` - Check if bot is responsive\n" +
                          $"• `/hello` - Get a friendly greeting\n" +
                          $"• `/info` - Show this information"
            });
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error while receiving message");
    }
};

// ============================================================================
// Additional Gateway Event Examples (Uncomment to use)
// ============================================================================

// Example: Log when the bot is ready
gateway.Ready += readyData =>
{
    try
    {
        Log.Information("Bot is ready! Logged in as {Username}", readyData.User?.Username);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error on ready event");
    }
};

// Example: Track message deletions
// gateway.MessageDelete += deleteData =>
// {
//     Log.Information("Message {MessageId} was deleted from channel {ChannelId}",
//                     deleteData.Id, deleteData.ChannelId);
// };

// Example: Welcome new guild members
// gateway.GuildMemberAdd += async memberData =>
// {
//     Log.Information("New member joined guild {GuildId}: User {UserId}",
//                     memberData.GuildId, memberData.UserId);
//
//     // Send a welcome message (replace with your welcome channel ID)
//     // await api.SendMessage(yourWelcomeChannelId, new()
//     // {
//     //     Content = $"Welcome to the server, <@{memberData.UserId}>! 🎉"
//     // });
// };

// Example: Track message reactions
// gateway.MessageReactionAdd += reactionData =>
// {
//     Log.Debug("Reaction {Emoji} added to message {MessageId} by user {UserId}",
//               reactionData.Emoji?.Name, reactionData.MessageId, reactionData.UserId);
// };

// ============================================================================
// STEP 8: Connect to the Gateway
// ============================================================================
// This establishes the WebSocket connection and starts receiving events.
// IMPORTANT: Uncomment this line to actually connect! It's commented out by
// default so you can test API calls without connecting to the gateway.

await gateway.ConnectAsync();
Log.Information("Connected to Fluxer gateway. Bot is now online!");

// ============================================================================
// STEP 9: Keep the Bot Running
// ============================================================================
// The bot needs to stay running to continue receiving events. Task.Delay(-1)
// blocks the main thread indefinitely. The bot will run until you stop it
// with Ctrl+C or kill the process.
//
// In production, you might want to:
//   - Add graceful shutdown handling (CancellationToken)
//   - Implement a /shutdown command for authorized users
//   - Run as a system service or Docker container

await api.UpdateCurrentMember(1431484523333775609, new() { Nickname = "Fluxer.Net" });

Log.Information("Bot is running. Press Ctrl+C to stop.");
await Task.Delay(-1);

// ============================================================================
// Next Steps & Resources
// ============================================================================
// Now that you understand the basics, here are some ideas to expand your bot:
//
// 1. Add more commands:
//    - Moderation commands (kick, ban, mute)
//    - Fun commands (random facts, jokes, games)
//    - Utility commands (polls, reminders, search)
//
// 2. Use more API endpoints:
//    - Create/manage channels: api.CreateChannel()
//    - Manage roles: api.CreateRole(), api.UpdateRole()
//    - Send embeds: Include embed data in SendMessage()
//    - Manage members: api.UpdateMember(), api.KickMember()
//
// 3. Implement advanced features:
//    - Command framework with prefix handling
//    - Permission checks before executing commands
//    - Database integration for persistent data
//    - Scheduled tasks and background jobs
//
// 4. Explore rate limiting:
//    - Check remaining requests: api.RateLimitManager.GetBucketInfoAsync()
//    - Monitor active buckets: api.RateLimitManager.ActiveBucketCount
//    - See RateLimiting/README.md for more details
//
// 5. Documentation:
//    - API endpoints: See ApiClient.cs for all 150+ methods
//    - Gateway events: See GatewayClient.cs for all event types
//    - Rate limiting: See RateLimiting/README.md
//    - Configuration: See FluxerConfig.cs for all options
//
// Happy coding! 🚀
// ============================================================================
