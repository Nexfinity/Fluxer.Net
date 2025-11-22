using Fluxer.Net.Commands;
using Fluxer.Net.Commands.Attributes;
using Fluxer.Net.Voice;
using Serilog;

namespace Fluxer.Net.Example.Modules;

/// <summary>
/// Commands for voice channel interactions using the Fluxer Voice Bridge.
///
/// Prerequisites:
/// 1. Install Node.js: https://nodejs.org/
/// 2. Navigate to FluxerVoiceBridge directory
/// 3. Run: npm install
/// 4. Start the bridge: npm start
/// </summary>
public class VoiceCommands : ModuleBase
{
	// Track active voice connection (simplified - in production use a proper state manager)
	private static VoiceBridgeClient? _voiceClient;
	private static bool _isMuted = false;
	private static bool _isDeafened = false;

	/// <summary>
	/// Join a voice channel.
	/// Usage: /join
	/// Note: You must be in a voice channel first!
	/// </summary>
	[Command("join")]
	[Summary("Join your current voice channel")]
	public async Task JoinCommand()
	{
		try
		{
			// Check if bridge is running
			if (!await CheckBridgeAvailableAsync())
			{
				await ReplyAsync("❌ Voice bridge is not running!\n" +
				                "Start the bridge first:\n" +
				                "1. cd FluxerVoiceBridge\n" +
				                "2. npm install\n" +
				                "3. npm start");
				return;
			}

			// Check if we have voice server info
			if (string.IsNullOrEmpty(VoiceStateManager.VoiceEndpoint) ||
			    string.IsNullOrEmpty(VoiceStateManager.VoiceToken))
			{
				await ReplyAsync("❌ No voice server information available.\n" +
				                "Make sure you're in a voice channel and try again!");
				return;
			}

			// Disconnect existing connection if any
			if (_voiceClient != null)
			{
				await _voiceClient.DisconnectAsync();
				_voiceClient = null;
			}

			await ReplyAsync("🔌 Connecting to voice channel...");

			// Create new voice bridge client
			_voiceClient = new VoiceBridgeClient(
				bridgeUrl: "ws://localhost:8765",
				guildId: VoiceStateManager.VoiceGuildId ?? 0,
				channelId: VoiceStateManager.VoiceChannelId ?? 0,
				userId: Context.User.Id,
				sessionId: VoiceStateManager.ConnectionId ?? "",
				logger: Log.Logger
			);

			// Set up event handlers
			_voiceClient.OnReady += () =>
			{
				Log.Information("✓ Voice connection ready!");
			};

			_voiceClient.OnConnected += async () =>
			{
				await ReplyAsync("✅ Successfully connected to voice channel!");
			};

			_voiceClient.OnDisconnected += async (reason) =>
			{
				await ReplyAsync($"🔌 Disconnected from voice: {reason}");
				_voiceClient = null;
				_isMuted = false;
				_isDeafened = false;
			};

			_voiceClient.OnParticipantJoined += async (participant) =>
			{
				Log.Information($"🎤 {participant.Identity} joined voice");
			};

			_voiceClient.OnParticipantLeft += async (identity) =>
			{
				Log.Information($"👋 {identity} left voice");
			};

			_voiceClient.OnSpeakingChanged += (speakers) =>
			{
				if (speakers.Length > 0)
					Log.Verbose($"🗣️ Speaking: {string.Join(", ", speakers)}");
			};

			_voiceClient.OnError += async (error) =>
			{
				Log.Error(error, "Voice error occurred");
				await ReplyAsync($"❌ Voice error: {error.Message}");
			};

			// Connect to LiveKit via bridge
			await _voiceClient.ConnectAsync(
				endpoint: VoiceStateManager.VoiceEndpoint!,
				token: VoiceStateManager.VoiceToken!
			);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error joining voice channel");
			await ReplyAsync($"❌ Error joining voice: {ex.Message}");
		}
	}

	/// <summary>
	/// Leave the current voice channel.
	/// Usage: /leave
	/// </summary>
	[Command("leave")]
	[Summary("Leave the current voice channel")]
	public async Task LeaveCommand()
	{
		if (_voiceClient == null)
		{
			await ReplyAsync("❌ Not connected to a voice channel!");
			return;
		}

		try
		{
			await _voiceClient.DisconnectAsync();
			_voiceClient = null;
			_isMuted = false;
			_isDeafened = false;
			await ReplyAsync("👋 Left voice channel");
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error leaving voice channel");
			await ReplyAsync($"❌ Error leaving voice: {ex.Message}");
		}
	}

	/// <summary>
	/// Toggle microphone mute.
	/// Usage: /mute
	/// </summary>
	[Command("mute")]
	[Summary("Toggle microphone mute")]
	public async Task MuteCommand()
	{
		if (_voiceClient == null)
		{
			await ReplyAsync("❌ Not connected to a voice channel!");
			return;
		}

		try
		{
			_isMuted = !_isMuted;
			await _voiceClient.SetMuteAsync(_isMuted);
			await ReplyAsync(_isMuted ? "🔇 Microphone muted" : "🔊 Microphone unmuted");
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error toggling mute");
			await ReplyAsync($"❌ Error toggling mute: {ex.Message}");
		}
	}

	/// <summary>
	/// Toggle deafen (mutes mic and disables audio output).
	/// Usage: /deaf
	/// </summary>
	[Command("deaf")]
	[Summary("Toggle deafen (mutes mic and disables audio)")]
	public async Task DeafCommand()
	{
		if (_voiceClient == null)
		{
			await ReplyAsync("❌ Not connected to a voice channel!");
			return;
		}

		try
		{
			_isDeafened = !_isDeafened;
			await _voiceClient.SetDeafAsync(_isDeafened);
			await ReplyAsync(_isDeafened ? "🔇 Deafened (mic muted, audio disabled)" : "🔊 Undeafened");
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error toggling deaf");
			await ReplyAsync($"❌ Error toggling deaf: {ex.Message}");
		}
	}

	/// <summary>
	/// Check if the voice bridge is running and accessible.
	/// </summary>
	private async Task<bool> CheckBridgeAvailableAsync()
	{
		try
		{
			using var testClient = new System.Net.WebSockets.ClientWebSocket();
			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
			await testClient.ConnectAsync(new Uri("ws://localhost:8765"), cts.Token);
			await testClient.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Test", CancellationToken.None);
			return true;
		}
		catch
		{
			return false;
		}
	}
}
