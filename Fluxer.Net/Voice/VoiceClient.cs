using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Websocket.Client;

namespace Fluxer.Net.Voice;

/// <summary>
/// Manages LiveKit voice connections for real-time audio streaming with Fluxer BETA API.
/// </summary>
/// <remarks>
/// This implementation uses LiveKit's WebRTC-based protocol, compatible with Fluxer BETA.
/// LiveKit uses a signaling server over WebSocket to establish WebRTC peer connections.
/// </remarks>
public class VoiceClient : IDisposable
{
	private readonly string _endpoint;
	private readonly ulong _guildId;
	private readonly ulong _userId;
	private readonly string _sessionId;
	private readonly string _token;
	private readonly ILogger? _logger;

	private WebsocketClient? _wsClient;
	private bool _isConnected;
	private string? _participantSid;
	private string? _roomName;

	public event Action? OnReady;
	public event Action<Exception>? OnError;

	/// <summary>
	/// Creates a new LiveKit VoiceClient instance.
	/// </summary>
	/// <param name="endpoint">LiveKit server endpoint (from VOICE_SERVER_UPDATE event, e.g., wss://ferret.iad.fluxer.media).</param>
	/// <param name="guildId">Guild/server ID.</param>
	/// <param name="channelId">Voice channel ID.</param>
	/// <param name="userId">Bot's user ID.</param>
	/// <param name="sessionId">Session ID (from VOICE_STATE_UPDATE event).</param>
	/// <param name="token">LiveKit JWT token (from VOICE_SERVER_UPDATE event).</param>
	/// <param name="logger">Optional Serilog logger.</param>
	public VoiceClient(string endpoint, ulong guildId, ulong channelId, ulong userId, string sessionId, string token, ILogger? logger = null)
	{
		// Remove the wss:// prefix if present to get just the hostname
		_endpoint = endpoint.Replace("wss://", "").Replace("ws://", "").Replace(":80", "").Replace(":443", "");
		_guildId = guildId;
		_userId = userId;
		_sessionId = sessionId;
		_token = token;
		_logger = logger;
		_roomName = $"guild_{guildId}_channel_{channelId}";
	}

	/// <summary>
	/// Connects to the LiveKit server and joins the voice room.
	/// </summary>
	public async Task ConnectAsync()
	{
		try
		{
			_logger?.Information("=== Starting LiveKit Connection ===");
			_logger?.Information("Endpoint: {Endpoint}", _endpoint);
			_logger?.Information("Room: {RoomName}", _roomName);
			_logger?.Information("User ID: {UserId}", _userId);
			_logger?.Information("Guild ID: {GuildId}", _guildId);
			_logger?.Information("Session ID: {SessionId}", _sessionId);
			_logger?.Information("Token length: {TokenLength} characters", _token?.Length ?? 0);

			// Construct LiveKit WebSocket URL with all required query parameters
			// Format from browser: wss://ferret.iad.fluxer.media/rtc?access_token=...&auto_subscribe=0&sdk=js&version=2.15.14&protocol=16&adaptive_stream=1
			var wsUrl = new Uri($"wss://{_endpoint}/rtc?access_token={_token}&auto_subscribe=0&sdk=js&version=2.15.14&protocol=16&adaptive_stream=1");

			_logger?.Information("Full WebSocket URL: wss://{Endpoint}/rtc?access_token={TokenPrefix}...&auto_subscribe=0&sdk=js&version=2.15.14&protocol=16&adaptive_stream=1",
				_endpoint, _token.Substring(0, Math.Min(20, _token.Length)));

			_logger?.Information("Creating WebSocket client...");
			_wsClient = new WebsocketClient(wsUrl);

			_wsClient.MessageReceived.Subscribe(HandleWebSocketMessage);
			_wsClient.DisconnectionHappened.Subscribe(info =>
			{
				_logger?.Warning("LiveKit WebSocket disconnected: {Type} - {Description}",
					info.Type, info.CloseStatusDescription);
				_isConnected = false;
			});
			_wsClient.ReconnectionHappened.Subscribe(info =>
			{
				_logger?.Information("LiveKit WebSocket reconnected: {Type}", info.Type);
			});

			_logger?.Information("Starting WebSocket connection...");
			await _wsClient.Start();
			_logger?.Information("WebSocket Start() completed - connection should be established");

			// Note: LiveKit uses Protocol Buffers for signaling, not JSON
			// The official LiveKit SDK handles the binary protobuf protocol
			// For now, we'll wait for server messages to see what format they use
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Failed to connect to LiveKit server");
			OnError?.Invoke(ex);
			throw;
		}
	}

	private async Task SendJoinRequest()
	{
		try
		{
			// LiveKit SignalRequest with JoinRequest
			// This is a simplified version - real LiveKit uses Protocol Buffers
			var joinRequest = new
			{
				join = new
				{
					room_name = _roomName,
					participant_name = $"bot_{_userId}",
					// Additional join parameters would go here
					auto_subscribe = true
				}
			};

			var json = JsonConvert.SerializeObject(joinRequest);
			_logger?.Debug("Sending LiveKit join request: {Json}", json);

			if (_wsClient?.IsRunning == true)
			{
				_wsClient.Send(json);
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error sending LiveKit join request");
		}
	}

	private void HandleWebSocketMessage(ResponseMessage message)
	{
		try
		{
			_logger?.Information("=== MESSAGE RECEIVED ===");
			_logger?.Information("Message type: {Type}", message.MessageType);

			// LiveKit uses Protocol Buffers for signaling
			if (message.MessageType == System.Net.WebSockets.WebSocketMessageType.Binary)
			{
				_logger?.Information("Received BINARY message of {Length} bytes", message.Binary?.Length ?? 0);

				if (message.Binary != null && message.Binary.Length > 0)
				{
					// Log first 100 bytes as hex for debugging
					var hexDump = BitConverter.ToString(message.Binary.Take(Math.Min(100, message.Binary.Length)).ToArray());
					_logger?.Debug("Binary data (first 100 bytes hex): {Hex}", hexDump);

					// Try to find readable strings in the binary data
					var readableChars = System.Text.Encoding.UTF8.GetString(message.Binary)
						.Where(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t')
						.ToArray();
					if (readableChars.Length > 0)
					{
						var readableText = new string(readableChars).Substring(0, Math.Min(200, readableChars.Length));
						_logger?.Debug("Readable text in binary: {Text}", readableText);
					}

					// For now, assume we successfully joined if we get any binary response
					// Full LiveKit implementation would require protobuf deserialization
					if (!_isConnected)
					{
						_logger?.Information("First binary message received - marking as connected");
						_logger?.Warning("Note: Full protobuf implementation needed for proper message handling");
						_isConnected = true;

						// Fire OnReady event
						try
						{
							OnReady?.Invoke();
							_logger?.Information("OnReady event invoked");
						}
						catch (Exception ex)
						{
							_logger?.Error(ex, "Error invoking OnReady event");
						}
					}
				}
			}
			else if (message.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
			{
				_logger?.Information("Received TEXT message: {Message}", message.Text?.Substring(0, Math.Min(500, message.Text?.Length ?? 0)));

				// Some implementations might use JSON for certain messages
				if (!string.IsNullOrEmpty(message.Text))
				{
					try
					{
						var json = JObject.Parse(message.Text);
						_logger?.Information("Parsed JSON message: {Json}", json.ToString(Formatting.None));

						// Check for various LiveKit message types
						if (json["join"] != null)
						{
							_logger?.Information("Received JOIN response!");
							_isConnected = true;
							_participantSid = json["join"]?["participant"]?["sid"]?.ToString();
							_logger?.Information("Participant SID: {Sid}", _participantSid);

							try
							{
								OnReady?.Invoke();
								_logger?.Information("OnReady event invoked");
							}
							catch (Exception ex)
							{
								_logger?.Error(ex, "Error invoking OnReady event");
							}
						}
						else if (json["offer"] != null)
						{
							_logger?.Information("Received WebRTC OFFER");
						}
						else if (json["answer"] != null)
						{
							_logger?.Information("Received WebRTC ANSWER");
						}
						else if (json["trickle"] != null)
						{
							_logger?.Debug("Received ICE candidate (trickle)");
						}
						else if (json["leave"] != null)
						{
							_logger?.Information("Received LEAVE notification");
						}
						else
						{
							_logger?.Warning("Received unknown JSON message type: {Keys}", string.Join(", ", json.Properties().Select(p => p.Name)));
						}
					}
					catch (JsonException ex)
					{
						_logger?.Warning(ex, "Text message is not valid JSON: {Text}", message.Text?.Substring(0, Math.Min(200, message.Text?.Length ?? 0)));
					}
				}
			}
			else
			{
				_logger?.Warning("Received message of unknown type: {Type}", message.MessageType);
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling LiveKit WebSocket message");
			OnError?.Invoke(ex);
		}
	}

	private void HandleJoinResponse(JToken joinResponse)
	{
		try
		{
			var room = joinResponse["room"];
			var participant = joinResponse["participant"];

			if (room != null)
			{
				_roomName = room["name"]?.ToString();
				_logger?.Information("Joined LiveKit room: {RoomName}", _roomName);
			}

			if (participant != null)
			{
				_participantSid = participant["sid"]?.ToString();
				_logger?.Information("LiveKit participant SID: {ParticipantSid}", _participantSid);
			}

			_isConnected = true;
			OnReady?.Invoke();
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling LiveKit join response");
		}
	}

	private void HandleParticipantUpdate(JToken participantUpdate)
	{
		try
		{
			var participants = participantUpdate["participants"];
			if (participants != null)
			{
				_logger?.Debug("LiveKit participant update: {Count} participants",
					((JArray)participants).Count);
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling LiveKit participant update");
		}
	}

	private void HandleTrackPublished(JToken trackPublished)
	{
		try
		{
			var trackSid = trackPublished["cid"]?.ToString();
			_logger?.Information("LiveKit track published: {TrackSid}", trackSid);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling LiveKit track published");
		}
	}

	/// <summary>
	/// Publishes an audio track to the LiveKit room.
	/// </summary>
	/// <param name="audioData">Raw audio data (PCM format recommended).</param>
	/// <param name="sampleRate">Sample rate (e.g., 48000).</param>
	/// <param name="channels">Number of audio channels (1 for mono, 2 for stereo).</param>
	public async Task PublishAudioTrackAsync(byte[] audioData, int sampleRate, int channels)
	{
		if (!_isConnected || _wsClient == null)
		{
			_logger?.Warning("Cannot publish audio track: not connected to LiveKit");
			return;
		}

		try
		{
			// Create add_track request
			var addTrackRequest = new
			{
				add_track = new
				{
					cid = $"audio_{Guid.NewGuid()}",
					name = "bot-audio",
					type = "AUDIO", // LiveKit TrackType
					source = "MICROPHONE", // LiveKit TrackSource
					// Audio encoding parameters
					width = 0,
					height = 0,
					muted = false,
					disable_dtx = false
				}
			};

			var json = JsonConvert.SerializeObject(addTrackRequest);
			_logger?.Debug("Publishing audio track: {Json}", json);

			_wsClient.Send(json);

			// Note: Actual audio streaming would require WebRTC data channels
			// or RTP streams, which is beyond this simplified implementation.
			// For full LiveKit support, you'd need the official SDK or WebRTC library.

			_logger?.Information("Audio track publish request sent (Note: actual audio streaming requires WebRTC data channel implementation)");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error publishing audio track");
			OnError?.Invoke(ex);
		}
	}

	/// <summary>
	/// Sends audio data over the voice connection.
	/// </summary>
	/// <param name="opusData">Opus-encoded audio frame.</param>
	/// <param name="opusLength">Length of Opus data.</param>
	/// <remarks>
	/// Note: This is a placeholder. Real LiveKit audio transmission requires WebRTC
	/// RTP streams or data channels, which need a full WebRTC implementation.
	/// Consider using SIPSorcery or another WebRTC library for production use.
	/// </remarks>
	public Task SendAudioAsync(byte[] opusData, int opusLength)
	{
		if (!_isConnected)
		{
			_logger?.Warning("Cannot send audio: not connected to LiveKit");
			return Task.CompletedTask;
		}

		// TODO: Implement WebRTC RTP audio streaming
		// This would require:
		// 1. WebRTC peer connection establishment
		// 2. DTLS handshake for encryption
		// 3. RTP packet construction with SRTP encryption
		// 4. Sending over UDP or WebRTC data channel

		_logger?.Debug("SendAudioAsync called (WebRTC audio streaming not yet implemented)");

		return Task.CompletedTask;
	}

	/// <summary>
	/// Sets the speaking state for the bot.
	/// </summary>
	/// <param name="speaking">True if speaking, false if not speaking.</param>
	/// <remarks>
	/// In LiveKit, speaking state is typically managed automatically by the SDK
	/// based on audio activity detection. This is a placeholder for compatibility.
	/// </remarks>
	public void SetSpeaking(bool speaking)
	{
		if (!_isConnected || _wsClient == null)
		{
			return;
		}

		try
		{
			// LiveKit manages speaking indicators automatically, but we can send
			// a metadata update if needed
			_logger?.Debug("Speaking state set to: {Speaking} (LiveKit auto-manages speaking indicators)", speaking);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error setting speaking state");
		}
	}

	/// <summary>
	/// Disconnects from the LiveKit server.
	/// </summary>
	public async Task DisconnectAsync()
	{
		try
		{
			_isConnected = false;

			// Send leave request
			if (_wsClient?.IsRunning == true)
			{
				var leaveRequest = new { leave = new { } };
				var json = JsonConvert.SerializeObject(leaveRequest);
				_wsClient.Send(json);

				await Task.Delay(100); // Give time for message to send
			}

			_wsClient?.Dispose();
			_logger?.Information("Disconnected from LiveKit server");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error disconnecting from LiveKit server");
		}
	}

	public void Dispose()
	{
		DisconnectAsync().Wait();
	}
}
