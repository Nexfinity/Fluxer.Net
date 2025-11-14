using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Fluxer.Net.Voice;

/// <summary>
/// Client for communicating with the Fluxer Voice Bridge (Node.js service).
/// This bridge uses the official LiveKit JavaScript SDK to handle all WebRTC complexity.
/// </summary>
public class VoiceBridgeClient : IDisposable
{
	private readonly string _bridgeUrl;
	private readonly ulong _guildId;
	private readonly ulong _channelId;
	private readonly ulong _userId;
	private readonly string _sessionId;
	private readonly ILogger? _logger;

	private ClientWebSocket? _ws;
	private bool _isConnected;
	private CancellationTokenSource? _cts;
	private Task? _receiveTask;

	public event Action? OnReady;
	public event Action? OnConnected;
	public event Action<string>? OnDisconnected;
	public event Action<ParticipantInfo>? OnParticipantJoined;
	public event Action<string>? OnParticipantLeft;
	public event Action<string[]>? OnSpeakingChanged;
	public event Action<Exception>? OnError;

	/// <summary>
	/// Creates a new VoiceBridgeClient instance.
	/// </summary>
	/// <param name="bridgeUrl">WebSocket URL of the Node.js bridge (e.g., "ws://localhost:8765")</param>
	/// <param name="guildId">Guild/server ID</param>
	/// <param name="channelId">Voice channel ID</param>
	/// <param name="userId">User ID</param>
	/// <param name="sessionId">Connection session ID</param>
	/// <param name="logger">Optional Serilog logger</param>
	public VoiceBridgeClient(string bridgeUrl, ulong guildId, ulong channelId, ulong userId, string sessionId, ILogger? logger = null)
	{
		_bridgeUrl = bridgeUrl;
		_guildId = guildId;
		_channelId = channelId;
		_userId = userId;
		_sessionId = sessionId;
		_logger = logger;
	}

	/// <summary>
	/// Connects to the voice bridge and joins the voice channel.
	/// </summary>
	/// <param name="endpoint">LiveKit server endpoint (from VoiceServerUpdate)</param>
	/// <param name="token">JWT token for authentication (from VoiceServerUpdate)</param>
	public async Task ConnectAsync(string endpoint, string token)
	{
		try
		{
			_logger?.Information("=== Connecting to Fluxer Voice Bridge ===");
			_logger?.Information("Bridge URL: {BridgeUrl}", _bridgeUrl);
			_logger?.Information("Guild ID: {GuildId}, Channel ID: {ChannelId}", _guildId, _channelId);
			_logger?.Information("User ID: {UserId}", _userId);

			// Connect to bridge WebSocket
			_ws = new ClientWebSocket();
			_cts = new CancellationTokenSource();

			var bridgeUri = new Uri(_bridgeUrl);
			await _ws.ConnectAsync(bridgeUri, _cts.Token);

			_isConnected = true;
			_logger?.Information("✓ Connected to voice bridge");

			// Start receiving messages
			_receiveTask = Task.Run(() => ReceiveLoop(_cts.Token));

			// Send connect command to bridge
			await SendMessageAsync(new
			{
				type = "CONNECT",
				connectionId = _sessionId,
				data = new
				{
					endpoint = endpoint,
					token = token,
					guildId = _guildId.ToString(),
					channelId = _channelId.ToString(),
					userId = _userId.ToString()
				}
			});

			_logger?.Information("✓ Sent CONNECT command to bridge");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Failed to connect to voice bridge");
			OnError?.Invoke(ex);
			throw;
		}
	}

	/// <summary>
	/// Disconnects from the voice channel.
	/// </summary>
	public async Task DisconnectAsync()
	{
		try
		{
			if (_ws?.State == WebSocketState.Open)
			{
				await SendMessageAsync(new
				{
					type = "DISCONNECT",
					connectionId = _sessionId
				});

				await Task.Delay(100); // Give bridge time to process disconnect
			}

			await CleanupAsync();
			_logger?.Information("Disconnected from voice bridge");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error disconnecting from voice bridge");
		}
	}

	/// <summary>
	/// Sets the mute state of the local microphone.
	/// </summary>
	public async Task SetMuteAsync(bool muted)
	{
		if (!_isConnected)
		{
			_logger?.Warning("Cannot set mute: not connected to bridge");
			return;
		}

		try
		{
			await SendMessageAsync(new
			{
				type = "SET_MUTE",
				connectionId = _sessionId,
				data = new { muted }
			});

			_logger?.Information("Set mute: {Muted}", muted);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error setting mute state");
		}
	}

	/// <summary>
	/// Sets the deafen state (mutes microphone and disables audio output).
	/// </summary>
	public async Task SetDeafAsync(bool deafened)
	{
		if (!_isConnected)
		{
			_logger?.Warning("Cannot set deaf: not connected to bridge");
			return;
		}

		try
		{
			await SendMessageAsync(new
			{
				type = "SET_DEAF",
				connectionId = _sessionId,
				data = new { deafened }
			});

			_logger?.Information("Set deaf: {Deafened}", deafened);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error setting deaf state");
		}
	}

	/// <summary>
	/// Receive loop for processing messages from the bridge.
	/// </summary>
	private async Task ReceiveLoop(CancellationToken cancellationToken)
	{
		var buffer = new byte[8192];

		try
		{
			while (_ws?.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
			{
				var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

				if (result.MessageType == WebSocketMessageType.Close)
				{
					_logger?.Warning("Bridge closed connection");
					_isConnected = false;
					break;
				}

				if (result.MessageType == WebSocketMessageType.Text)
				{
					var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
					HandleMessage(text);
				}
			}
		}
		catch (OperationCanceledException)
		{
			_logger?.Debug("Receive loop cancelled");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error in receive loop");
			OnError?.Invoke(ex);
		}
	}

	/// <summary>
	/// Handle incoming messages from the bridge.
	/// </summary>
	private void HandleMessage(string text)
	{
		try
		{
			var json = JObject.Parse(text);
			var type = json["type"]?.ToString();
			var data = json["data"] as JObject;

			_logger?.Verbose("Received: {Type}", type);

			switch (type)
			{
				case "READY":
					_logger?.Information("✓ Voice connection ready");
					_logger?.Information("  Room: {RoomName}", data?["roomName"]);
					_logger?.Information("  Participants: {Count}", data?["participantCount"]);
					OnReady?.Invoke();
					break;

				case "CONNECTED":
					_logger?.Information("✓ Voice connection established");
					OnConnected?.Invoke();
					break;

				case "DISCONNECTED":
					var reason = data?["reason"]?.ToString() ?? "Unknown";
					_logger?.Warning("Voice connection disconnected: {Reason}", reason);
					_isConnected = false;
					OnDisconnected?.Invoke(reason);
					break;

				case "RECONNECTING":
					_logger?.Information("Voice connection reconnecting...");
					break;

				case "RECONNECTED":
					_logger?.Information("✓ Voice connection reconnected");
					break;

				case "PARTICIPANT_JOINED":
					if (data != null)
					{
						var participant = JsonConvert.DeserializeObject<ParticipantInfo>(data.ToString());
						if (participant != null)
						{
							_logger?.Information("Participant joined: {Identity}", participant.Identity);
							OnParticipantJoined?.Invoke(participant);
						}
					}
					break;

				case "PARTICIPANT_LEFT":
					var identity = data?["identity"]?.ToString();
					if (identity != null)
					{
						_logger?.Information("Participant left: {Identity}", identity);
						OnParticipantLeft?.Invoke(identity);
					}
					break;

				case "SPEAKING_CHANGED":
					var speakers = data?["speakers"]?.ToObject<string[]>();
					if (speakers != null)
					{
						_logger?.Verbose("Speaking changed: {Speakers}", string.Join(", ", speakers));
						OnSpeakingChanged?.Invoke(speakers);
					}
					break;

				case "TRACK_SUBSCRIBED":
					_logger?.Debug("Track subscribed: {Kind} from {Participant}",
						data?["kind"], data?["participant"]);
					break;

				case "TRACK_UNSUBSCRIBED":
					_logger?.Debug("Track unsubscribed from {Participant}", data?["participant"]);
					break;

				case "CONNECTION_QUALITY":
					_logger?.Verbose("Connection quality: {Quality} for {Participant}",
						data?["quality"], data?["participant"]);
					break;

				case "MUTE_CHANGED":
					_logger?.Information("Mute state changed: {Muted}", data?["muted"]);
					break;

				case "DEAF_CHANGED":
					_logger?.Information("Deaf state changed: {Deafened}", data?["deafened"]);
					break;

				case "ERROR":
					var code = data?["code"]?.ToString();
					var message = data?["message"]?.ToString();
					_logger?.Error("Bridge error: {Code} - {Message}", code, message);
					OnError?.Invoke(new Exception($"{code}: {message}"));
					break;

				case "PONG":
					_logger?.Verbose("Received PONG");
					break;

				default:
					_logger?.Debug("Unknown message type: {Type}", type);
					break;
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling message: {Text}", text.Substring(0, Math.Min(100, text.Length)));
		}
	}

	/// <summary>
	/// Send a message to the bridge.
	/// </summary>
	private async Task SendMessageAsync(object message)
	{
		if (_ws?.State != WebSocketState.Open)
		{
			throw new InvalidOperationException("WebSocket is not connected");
		}

		var json = JsonConvert.SerializeObject(message);
		var bytes = Encoding.UTF8.GetBytes(json);
		await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
	}

	/// <summary>
	/// Cleanup resources.
	/// </summary>
	private async Task CleanupAsync()
	{
		_isConnected = false;

		_cts?.Cancel();

		if (_ws?.State == WebSocketState.Open)
		{
			try
			{
				await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
			}
			catch { }
		}

		_ws?.Dispose();
		_cts?.Dispose();

		if (_receiveTask != null)
		{
			try
			{
				await _receiveTask;
			}
			catch { }
		}
	}

	public void Dispose()
	{
		DisconnectAsync().Wait();
	}
}

/// <summary>
/// Information about a participant in a voice channel.
/// </summary>
public class ParticipantInfo
{
	[JsonProperty("identity")]
	public string Identity { get; set; } = "";

	[JsonProperty("sid")]
	public string Sid { get; set; } = "";

	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("metadata")]
	public string? Metadata { get; set; }

	[JsonProperty("isSpeaking")]
	public bool IsSpeaking { get; set; }

	[JsonProperty("connectionQuality")]
	public string ConnectionQuality { get; set; } = "";

	[JsonProperty("isMicrophoneEnabled")]
	public bool IsMicrophoneEnabled { get; set; }

	[JsonProperty("isCameraEnabled")]
	public bool IsCameraEnabled { get; set; }

	[JsonProperty("isScreenShareEnabled")]
	public bool IsScreenShareEnabled { get; set; }
}
