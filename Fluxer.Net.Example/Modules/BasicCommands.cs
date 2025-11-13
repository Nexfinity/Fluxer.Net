using Fluxer.Net.Commands;
using Fluxer.Net.Commands.Attributes;
using Fluxer.Net.Data.Models;
using Fluxer.Net.EmbedBuilder;
using Fluxer.Net.Voice;
using Serilog;
using Serilog.Core;

namespace Fluxer.Net.Example.Modules;

/// <summary>
/// Example command module demonstrating basic commands.
/// </summary>
public class BasicCommands : ModuleBase
{
	/// <summary>
	/// Simple ping command that responds with "pong".
	/// </summary>
	[Command("ping")]
	[Summary("Check if the bot is responsive")]
	public async Task PingCommand()
	{
		await ReplyAsync("pong ;P");
	}

	/// <summary>
	/// Hello command that mentions the user.
	/// </summary>
	[Command("hello")]
	[Alias("hi", "hey")]
	[Summary("Get a friendly greeting")]
	public async Task HelloCommand()
	{
		await ReplyAsync($"Hello, <@{Context.User.Id}>! 👋");
	}

	/// <summary>
	/// Info command that shows bot information and available commands.
	/// </summary>
	[Command("info")]
	[Summary("Show bot information and available commands")]
	public async Task InfoCommand()
	{
		await ReplyAsync(
			$"**Fluxer.Net Example Bot**\n" +
			$"Version: 0.4.0\n" +
			$"Framework: .NET 7.0\n" +
			$"Library: Fluxer.Net\n\n" +
			$"Available Commands:\n" +
			$"• `/ping` - Check if bot is responsive\n" +
			$"• `/hello` - Get a friendly greeting\n" +
			$"• `/info` - Show this information\n" +
			$"• `/embed` - Show an example rich embed\n" +
			$"• `/echo <message>` - Echo back your message\n" +
			$"• `/add <a> <b>` - Add two numbers"
		);
	}

	/// <summary>
	/// Embed command that demonstrates rich embeds using EmbedBuilder.
	/// </summary>
	[Command("embed")]
	[Summary("Show an example rich embed")]
	public async Task EmbedCommand()
	{
		var embed = new Fluxer.Net.EmbedBuilder.EmbedBuilder()
			.WithTitle("Example Rich Embed")
			.WithDescription("This is a demonstration of Fluxer.Net's EmbedBuilder system, " +
							 "based on Discord.Net's implementation. Embeds support rich formatting " +
							 "with titles, descriptions, fields, images, and more!")
			.WithColor(0x5865F2) // Blurple color
			.WithAuthor(
				name: Context.User.Username,
				iconUrl: Context.User.Avatar != null
					? $"https://cdn.fluxer.dev/avatars/{Context.User.Id}/{Context.User.Avatar}.png"
					: null
			)
			.WithThumbnailUrl("https://avatars.githubusercontent.com/u/20194446")
			.AddField("Field 1", "This is an inline field", inline: true)
			.AddField("Field 2", "This is also inline", inline: true)
			.AddField("Field 3", "This is another inline field", inline: true)
			.AddField("Full Width Field", "This field takes up the full width because inline is false", inline: false)
			.AddField("Bot Stats", $"Guilds: 1\nChannels: 5\nUptime: {DateTime.UtcNow:HH:mm:ss}", inline: true)
			.WithFooter("Fluxer.Net v0.4.0", "https://avatars.githubusercontent.com/u/20194446")
			.WithCurrentTimestamp()
			.Build();

		await Context.Client.SendMessage(Context.ChannelId, new()
		{
			Content = "Here's an example of a rich embed:",
			Embeds = new List<Embed> { embed }
		});
	}

	/// <summary>
	/// Echo command that repeats the user's message.
	/// </summary>
	[Command("echo")]
	[Summary("Echo back your message")]
	public async Task EchoCommand([Remainder] string message)
	{
		await ReplyAsync(message);
	}

	/// <summary>
	/// Add command that adds two numbers together.
	/// </summary>
	[Command("add")]
	[Summary("Add two numbers together")]
	public async Task AddCommand(int a, int b)
	{
		await ReplyAsync($"{a} + {b} = {a + b}");
	}

	/// <summary>
	/// Example command with optional parameter.
	/// </summary>
	[Command("greet")]
	[Summary("Greet someone (or yourself)")]
	public async Task GreetCommand(string name = "stranger")
	{
		await ReplyAsync($"Hello, {name}!");
	}

	/// <summary>
	/// An example voice chat command.
	/// </summary>
	[Command("play")]
	[Summary("Play a test song.")]
	public async Task PlayCommand(ulong voiceChannelId)
	{
		// Use hardcoded crab-rave.mp3 file
		string filePath = "crab-rave.mp3";
		if (!File.Exists(filePath))
		{
			await ReplyAsync($"File not found: `{filePath}` - Please place crab-rave.mp3 in the same directory as the executable.");
			return;
		}

		await ReplyAsync($"Joining voice channel and playing: `{Path.GetFileName(filePath)}`...");

		ulong guildId = 1431484523333775609; // Replace with your guild ID

		try
		{
			// Reset voice state before joining
			VoiceStateManager.Reset();

			// Send OpCode 4 (VOICE_STATE_UPDATE) to join the voice channel
			// The gateway will respond with VOICE_SERVER_UPDATE and VOICE_STATE_UPDATE events
			Log.Information("=== Sending OpCode 4 VOICE_STATE_UPDATE ===");
			Log.Information("Guild ID: {GuildId}", guildId);
			Log.Information("Channel ID: {ChannelId}", voiceChannelId);
			Context.Gateway.UpdateVoiceState(guildId, voiceChannelId, false, false);

			// Wait for gateway events to populate VoiceStateManager
			var timeout = DateTime.UtcNow.AddSeconds(10);
			while (!VoiceStateManager.IsVoiceDataReady() && DateTime.UtcNow < timeout)
			{
				await Task.Delay(100);
			}

			Log.Information("=== Voice connection data after gateway events ===");
			Log.Information("Endpoint: {Endpoint}", VoiceStateManager.VoiceEndpoint);
			Log.Information("Token present: {HasToken}", VoiceStateManager.VoiceToken != null);
			Log.Information("Session ID: {SessionId}", VoiceStateManager.VoiceSessionId);
			Log.Information("Channel ID: {ChannelId}", VoiceStateManager.VoiceChannelId);
			Log.Information("Connection ID: {ConnectionId}", VoiceStateManager.ConnectionId);
			Log.Information("User present: {HasUser}", VoiceStateManager.ReadyData?.User != null);

			if (!VoiceStateManager.IsVoiceDataReady())
			{
				await ReplyAsync("Failed to join voice channel: Timeout waiting for gateway events (VOICE_SERVER_UPDATE/VOICE_STATE_UPDATE).");
				return;
			}

			if (!VoiceStateManager.VoiceChannelId.HasValue)
			{
				await ReplyAsync("Failed to join voice channel: Channel ID not received from gateway.");
				return;
			}

			// Create voice client with credentials from gateway events
			var voiceClient = new VoiceClient(
				endpoint: VoiceStateManager.VoiceEndpoint!,
				guildId: guildId,
				channelId: VoiceStateManager.VoiceChannelId.Value,
				userId: VoiceStateManager.ReadyData!.User!.Id,
				sessionId: VoiceStateManager.VoiceSessionId!,
				token: VoiceStateManager.VoiceToken!,
				logger: Log.Logger as Logger
			);

			// Connect to LiveKit WebSocket
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
				await ReplyAsync("Failed to establish LiveKit WebSocket connection.");
				voiceClient.Dispose();
				return;
			}

			await ReplyAsync("Successfully connected to voice channel!");

			// Play audio
			var audioPlayer = new AudioPlayer(voiceClient, Log.Logger as Logger);
			audioPlayer.OnPlaybackFinished += async () =>
			{
				await ReplyAsync("Playback finished!");
				await voiceClient.DisconnectAsync();
				voiceClient.Dispose();
			};

			audioPlayer.OnError += async (error) =>
			{
				Log.Error(error, "Audio playback error");
				await ReplyAsync($"Playback error: {error.Message}");
				await voiceClient.DisconnectAsync();
				voiceClient.Dispose();
			};

			await audioPlayer.PlayAsync(filePath);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error playing audio");
			await ReplyAsync($"Error: {ex.Message}");
		}
	}
}
