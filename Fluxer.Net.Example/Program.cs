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
using Fluxer.Net.EmbedBuilder;
using Fluxer.Net.Example;
using Fluxer.Net.Gateway.Data;
using Fluxer.Net.Voice;

// ============================================================================
// STEP 1: Configure Logging
// ============================================================================
// Serilog provides structured logging for the Fluxer.Net library. This helps
// you debug issues and monitor your bot's activity. Logs are written to both
// the console (for development) and a file (for production debugging).

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()  // Log everything (Verbose, Debug, Info, Warning, Error, Fatal)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)  // Pretty console output with colors
                .WriteTo.File($"output-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log",
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
// STEP 7: Voice State Tracking Variables
// ============================================================================
// These variables track voice connection state and must be declared before
// the event handlers that use them.

string? voiceEndpoint = null;
string? voiceToken = null;
string? voiceSessionId = null;
ulong? voiceGuildId = null;
ReadyGatewayData? readyData = null;

// ============================================================================
// STEP 8: Subscribe to Gateway Events
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
                          $"• `/info` - Show this information\n" +
                          $"• `/embed` - Show an example rich embed\n" +
                          $"• `/play <channel_id>` - Join voice and play crab-rave.mp3"
            });
        }

        // ========================================================================
        // Example Command: /embed
        // ========================================================================
        // Demonstrates using the EmbedBuilder to create rich embeds

        else if (messageData.Content == "/embed")
        {
            // Create a rich embed using the fluent EmbedBuilder API
            var embed = new EmbedBuilder()
                .WithTitle("Example Rich Embed")
                .WithDescription("This is a demonstration of Fluxer.Net's EmbedBuilder system, " +
                                 "based on Discord.Net's implementation. Embeds support rich formatting " +
                                 "with titles, descriptions, fields, images, and more!")
                .WithColor(0x5865F2) // Blurple color (RGB: 88, 101, 242)
                .WithAuthor(
                    name: messageData.Author.Username,
                    iconUrl: messageData.Author.Avatar != null
                        ? $"https://cdn.fluxer.dev/avatars/{messageData.Author.Id}/{messageData.Author.Avatar}.png"
                        : null
                )
                .WithThumbnailUrl("https://avatars.githubusercontent.com/u/20194446")
                .WithImageUrl("https://repository-images.githubusercontent.com/123456789/example")
                .AddField("Field 1", "This is an inline field", inline: true)
                .AddField("Field 2", "This is also inline", inline: true)
                .AddField("Field 3", "This is another inline field", inline: true)
                .AddField("Full Width Field", "This field takes up the full width because inline is false", inline: false)
                .AddField("Bot Stats", $"Guilds: 1\nChannels: 5\nUptime: {DateTime.UtcNow:HH:mm:ss}", inline: true)
                .WithFooter("Fluxer.Net v0.4.0", "https://avatars.githubusercontent.com/u/20194446")
                .WithCurrentTimestamp()
                .Build();

            await api.SendMessage(messageData.ChannelId, new()
            {
                Content = "Here's an example of a rich embed:",
                Embeds = new List<Fluxer.Net.Data.Models.Embed> { embed }
            });
        }

        // ========================================================================
        // Example Command: /play <voice_channel_id>
        // ========================================================================
        // Demonstrates joining a voice channel and playing crab-rave.mp3
        // Usage: /play 123456789

        else if (messageData.Content.StartsWith("/play "))
        {
            var parts = messageData.Content.Split(' ', 2);
            if (parts.Length < 2)
            {
                await api.SendMessage(messageData.ChannelId, new()
                {
                    Content = "Usage: `/play <voice_channel_id>`\n" +
                              "Example: `/play 123456789`"
                });
                return;
            }

            if (!ulong.TryParse(parts[1], out ulong voiceChannelId))
            {
                await api.SendMessage(messageData.ChannelId, new()
                {
                    Content = "Invalid voice channel ID. Please provide a valid numeric channel ID."
                });
                return;
            }

            // Use hardcoded crab-rave.mp3 file
            string filePath = "crab-rave.mp3";
            if (!File.Exists(filePath))
            {
                await api.SendMessage(messageData.ChannelId, new()
                {
                    Content = $"File not found: `{filePath}` - Please place crab-rave.mp3 in the same directory as the executable."
                });
                return;
            }

            await api.SendMessage(messageData.ChannelId, new()
            {
                Content = $"Joining voice channel and playing: `{Path.GetFileName(filePath)}`..."
            });

            // Join voice channel by sending Voice State Update via gateway
            // Note: In production, you'd need to track which guild the channel belongs to
            // For this example, we'll use a fixed guild ID
            ulong guildId = 1431484523333775609; // Replace with your guild ID

            try
            {
                // Send voice state update to join the channel
                gateway.UpdateVoiceState(guildId, voiceChannelId, false, false);

                // Wait for voice server and state updates (with timeout)
                var timeout = DateTime.UtcNow.AddSeconds(10);
                while ((voiceEndpoint == null || voiceToken == null || voiceSessionId == null) &&
                       DateTime.UtcNow < timeout)
                {
                    await Task.Delay(100);
                }

                if (voiceEndpoint == null || voiceToken == null || voiceSessionId == null || readyData?.User == null)
                {
                    await api.SendMessage(messageData.ChannelId, new()
                    {
                        Content = "Failed to join voice channel: Timeout waiting for voice connection data."
                    });
                    return;
                }

                // Create voice client
                var voiceClient = new VoiceClient(
                    endpoint: voiceEndpoint,
                    guildId: guildId,
                    userId: readyData.User.Id,
                    sessionId: voiceSessionId,
                    token: voiceToken,
                    logger: Log.Logger as Logger
                );

                // Connect to voice
                await voiceClient.ConnectAsync();

                // Wait for voice client to be ready
                var voiceReady = false;
                voiceClient.OnReady += () => { voiceReady = true; };

                timeout = DateTime.UtcNow.AddSeconds(10);
                while (!voiceReady && DateTime.UtcNow < timeout)
                {
                    await Task.Delay(100);
                }

                if (!voiceReady)
                {
                    await api.SendMessage(messageData.ChannelId, new()
                    {
                        Content = "Failed to establish voice connection."
                    });
                    voiceClient.Dispose();
                    return;
                }

                // Play audio
                var audioPlayer = new AudioPlayer(voiceClient, Log.Logger as Logger);
                audioPlayer.OnPlaybackFinished += async () =>
                {
                    await api.SendMessage(messageData.ChannelId, new()
                    {
                        Content = "Playback finished!"
                    });
                    await voiceClient.DisconnectAsync();
                    voiceClient.Dispose();
                };

                audioPlayer.OnError += async (error) =>
                {
                    Log.Error(error, "Audio playback error");
                    await api.SendMessage(messageData.ChannelId, new()
                    {
                        Content = $"Playback error: {error.Message}"
                    });
                    await voiceClient.DisconnectAsync();
                    voiceClient.Dispose();
                };

                await audioPlayer.PlayAsync(filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error playing audio");
                await api.SendMessage(messageData.ChannelId, new()
                {
                    Content = $"Error: {ex.Message}"
                });
            }
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error while receiving message");
    }
};

// ============================================================================
// Additional Gateway Event Handlers
// ============================================================================

// Voice State Tracking - Handle voice server and state updates
gateway.VoiceServerUpdate += voiceData =>
{
    voiceEndpoint = voiceData.Endpoint;
    voiceToken = voiceData.Token;
    voiceGuildId = voiceData.GuildId;
    Log.Debug("Voice server update: Endpoint={Endpoint}, Guild={GuildId}", voiceEndpoint, voiceGuildId);
};

gateway.VoiceStateUpdate += voiceStateData =>
{
    if (voiceStateData.UserId.ToString() == readyData?.User?.Id.ToString())
    {
        voiceSessionId = voiceStateData.SessionId;
        Log.Debug("Voice state update: Session={SessionId}, Channel={ChannelId}", voiceSessionId, voiceStateData.ChannelId);
    }
};

// Log when the bot is ready
gateway.Ready += data =>
{
    try
    {
        readyData = data;
        Log.Information("Bot is ready! Logged in as {Username}", data.User?.Username);
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
//    - Send embeds: Use EmbedBuilder to create rich embeds (see /embed command)
//    - Manage members: api.UpdateMember(), api.KickMember()
//
//    Example: Create a complex embed with error handling
//    try {
//        var embed = new EmbedBuilder()
//            .WithTitle("Server Stats")
//            .WithDescription($"Statistics for {guildName}")
//            .WithColor(0x00FF00) // Green
//            .AddField("Total Members", memberCount.ToString(), inline: true)
//            .AddField("Online Members", onlineCount.ToString(), inline: true)
//            .AddField("Total Channels", channelCount.ToString(), inline: true)
//            .WithThumbnailUrl(guildIconUrl)
//            .WithFooter($"Requested by {username}", userAvatarUrl)
//            .WithCurrentTimestamp()
//            .Build();
//
//        await api.SendMessage(channelId, new() { Embeds = new() { embed } });
//    } catch (InvalidOperationException ex) {
//        Log.Error(ex, "Embed validation failed - check field lengths and URL formats");
//    }
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
