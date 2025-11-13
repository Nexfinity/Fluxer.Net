using System.Text;
using Fluxer.Net.Voice.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using SIPSorcery.Net;
using Websocket.Client;

namespace Fluxer.Net.Voice;

/// <summary>
/// Manages LiveKit voice connections for real-time audio streaming with Fluxer BETA API.
/// Uses minimal protobuf parsing and SIPSorcery for WebRTC.
/// </summary>
public class VoiceClient : IDisposable
{
	private readonly string _endpoint;
	private readonly ulong _guildId;
	private readonly ulong _channelId;
	private readonly ulong _userId;
	private readonly string _sessionId;
	private readonly string _token;
	private readonly ILogger? _logger;

	private WebsocketClient? _wsClient;
	private RTCPeerConnection? _peerConnection;
	private bool _isConnected;
	private bool _isWebRTCConnected;
	private System.Timers.Timer? _pingTimer;
	private readonly List<string> _pendingIceCandidates = new List<string>();

	public event Action? OnReady;
	public event Action<Exception>? OnError;

	/// <summary>
	/// Creates a new LiveKit VoiceClient instance.
	/// </summary>
	public VoiceClient(string endpoint, ulong guildId, ulong channelId, ulong userId, string sessionId, string token, ILogger? logger = null)
	{
		// Remove the wss:// prefix if present
		_endpoint = endpoint.Replace("wss://", "").Replace("ws://", "").Replace(":80", "").Replace(":443", "");
		_guildId = guildId;
		_channelId = channelId;
		_userId = userId;
		_sessionId = sessionId;
		_token = token;
		_logger = logger;
	}

	/// <summary>
	/// Connects to the LiveKit server and joins the voice room.
	/// </summary>
	public async Task ConnectAsync()
	{
		try
		{
			_logger?.Information("=== Starting LiveKit Connection (Minimal Implementation) ===");
			_logger?.Information("Endpoint: {Endpoint}", _endpoint);
			_logger?.Information("Guild ID: {GuildId}, Channel ID: {ChannelId}", _guildId, _channelId);
			_logger?.Information("User ID: {UserId}", _userId);
			_logger?.Information("Session ID: {SessionId}", _sessionId);
			_logger?.Information("Token length: {TokenLength} characters", _token?.Length ?? 0);

			// Construct LiveKit WebSocket URL
			var wsUrl = new Uri($"wss://{_endpoint}/rtc?access_token={_token}&auto_subscribe=0&sdk=js&version=2.15.14&protocol=16&adaptive_stream=1");

			_logger?.Information("Creating WebSocket client...");
			_wsClient = new WebsocketClient(wsUrl);

			_wsClient.MessageReceived.Subscribe(HandleWebSocketMessage);
			_wsClient.DisconnectionHappened.Subscribe(info =>
			{
				_logger?.Warning("LiveKit WebSocket disconnected: {Type} - {Description}",
					info.Type, info.CloseStatusDescription);
				_isConnected = false;
				_isWebRTCConnected = false;
			});
			_wsClient.ReconnectionHappened.Subscribe(info =>
			{
				_logger?.Information("LiveKit WebSocket reconnected: {Type}", info.Type);
			});

			_logger?.Information("Starting WebSocket connection...");
			await _wsClient.Start();
			_logger?.Information("WebSocket connected, waiting for server messages...");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Failed to connect to LiveKit server");
			OnError?.Invoke(ex);
			throw;
		}
	}

	private void HandleWebSocketMessage(ResponseMessage message)
	{
		try
		{
			_logger?.Verbose("=== MESSAGE RECEIVED ===");
			_logger?.Verbose("Message type: {Type}", message.MessageType);

			if (message.MessageType == System.Net.WebSockets.WebSocketMessageType.Binary && message.Binary != null)
			{
				_logger?.Verbose("Received BINARY message of {Length} bytes", message.Binary.Length);
				HandleBinaryMessage(message.Binary);
			}
			else if (message.MessageType == System.Net.WebSockets.WebSocketMessageType.Text && !string.IsNullOrEmpty(message.Text))
			{
				_logger?.Information("Received TEXT message: {Message}", message.Text.Substring(0, Math.Min(200, message.Text.Length)));
				HandleTextMessage(message.Text);
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling LiveKit WebSocket message");
			OnError?.Invoke(ex);
		}
	}

	private void HandleBinaryMessage(byte[] data)
	{
		// Check if this is a JoinResponse
		if (SimpleProtobufParser.IsJoinResponse(data))
		{
			_logger?.Information("✓ Received JoinResponse from server");
			_isConnected = true;
			return;
		}

		// Try to extract SDP offer
		var sdp = SimpleProtobufParser.TryExtractSDP(data);
		if (!string.IsNullOrEmpty(sdp))
		{
			_logger?.Information("✓ Received SDP Offer from server ({Length} chars)", sdp.Length);
			_logger?.Debug("SDP Offer preview: {SDP}", sdp.Substring(0, Math.Min(200, sdp.Length)));
			_ = Task.Run(() => HandleOfferAsync(sdp));
			return;
		}

		// Try to extract ICE candidate
		var candidate = SimpleProtobufParser.TryExtractIceCandidate(data);
		if (!string.IsNullOrEmpty(candidate))
		{
			_logger?.Debug("✓ Received remote ICE candidate from server");
			HandleRemoteIceCandidate(candidate);
			return;
		}

		// Unknown message type - log for debugging
		_logger?.Debug("Received unhandled binary message ({Length} bytes)", data.Length);
	}

	private void HandleTextMessage(string text)
	{
		try
		{
			var json = JObject.Parse(text);

			if (json["leave"] != null)
			{
				_logger?.Warning("✗ Received LEAVE notification from server");
				_isConnected = false;
				_isWebRTCConnected = false;
			}
			else if (json["pong"] != null)
			{
				_logger?.Verbose("✓ Received PONG from server");
			}
			else
			{
				_logger?.Debug("Received unknown JSON message: {Keys}",
					string.Join(", ", json.Properties().Select(p => p.Name)));
			}
		}
		catch (JsonException)
		{
			_logger?.Debug("Received non-JSON text message: {Text}",
				text.Substring(0, Math.Min(100, text.Length)));
		}
	}

	private async Task HandleOfferAsync(string sdp)
	{
		try
		{
			_logger?.Information("=== Handling WebRTC Offer ===");

			// Create peer connection
			_peerConnection = new RTCPeerConnection(new RTCConfiguration
			{
				iceServers = new List<RTCIceServer>
				{
					new RTCIceServer
					{
						urls = "stun:stun.l.google.com:19302"
					}
				}
			});

			// Add audio track for voice communication
			// Use Opus codec (96) which is what LiveKit expects for audio
			var audioFormat = new SDPAudioVideoMediaFormat(SDPMediaTypesEnum.audio, 96, "opus", 48000, 2);
			var audioTrack = new MediaStreamTrack(SDPMediaTypesEnum.audio, false, new List<SDPAudioVideoMediaFormat> { audioFormat });
			_peerConnection.addTrack(audioTrack);
			_logger?.Debug("✓ Added audio track to peer connection (Opus codec)");

			// Set up ICE candidate handler
			_peerConnection.onicecandidate += (candidate) =>
			{
				if (candidate != null)
				{
					_logger?.Debug("✓ Local ICE candidate generated: {Candidate}",
						candidate.ToString().Substring(0, Math.Min(100, candidate.ToString().Length)));
					SendIceCandidate(candidate);
				}
			};

			// Set up connection state handler
			_peerConnection.onconnectionstatechange += (state) =>
			{
				_logger?.Information("WebRTC connection state: {State}", state);

				if (state == RTCPeerConnectionState.connected)
				{
					_isWebRTCConnected = true;
					_logger?.Information("✓✓✓ WebRTC connection established! ✓✓✓");

					// Start ping timer to keep connection alive
					StartPingTimer();

					// Fire OnReady event
					try
					{
						OnReady?.Invoke();
					}
					catch (Exception ex)
					{
						_logger?.Error(ex, "Error invoking OnReady event");
					}
				}
				else if (state == RTCPeerConnectionState.failed || state == RTCPeerConnectionState.disconnected)
				{
					_isWebRTCConnected = false;
					_logger?.Warning("✗ WebRTC connection lost");
				}
			};

			// Set remote description (the offer)
			_logger?.Information("Setting remote description (offer)...");
			var rtcOffer = new RTCSessionDescriptionInit
			{
				type = RTCSdpType.offer,
				sdp = sdp
			};

			_peerConnection.setRemoteDescription(rtcOffer);
			_logger?.Information("✓ Remote description set successfully");

			// Create answer
			_logger?.Information("Creating SDP answer...");
			var answerInit = _peerConnection.createAnswer(null);
			if (answerInit == null)
			{
				_logger?.Error("Failed to create answer");
				return;
			}

			_logger?.Information("✓ SDP answer created");

			// Set local description
			_peerConnection.setLocalDescription(answerInit);
			_logger?.Information("✓ Local description set successfully");

			// Send answer back to server via protobuf
			var answerBytes = SimpleProtobufParser.CreateAnswerRequest(answerInit.sdp);
			_wsClient?.Send(answerBytes);

			_logger?.Information("✓ Answer sent to LiveKit server ({Length} bytes)", answerBytes.Length);
			_logger?.Debug("Answer SDP preview: {SDP}",
				answerInit.sdp.Substring(0, Math.Min(200, answerInit.sdp.Length)));

			// Process any ICE candidates that arrived before the peer connection was created
			lock (_pendingIceCandidates)
			{
				if (_pendingIceCandidates.Count > 0)
				{
					_logger?.Information("Processing {Count} queued ICE candidates", _pendingIceCandidates.Count);
					foreach (var candidate in _pendingIceCandidates)
					{
						try
						{
							var rtcCandidate = new RTCIceCandidateInit { candidate = candidate };
							_peerConnection.addIceCandidate(rtcCandidate);
							_logger?.Verbose("✓ Added queued remote ICE candidate");
						}
						catch (Exception ex)
						{
							_logger?.Warning(ex, "Error adding queued ICE candidate");
						}
					}
					_pendingIceCandidates.Clear();
				}
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling offer");
			OnError?.Invoke(ex);
		}
	}

	private void HandleRemoteIceCandidate(string candidateInit)
	{
		try
		{
			if (_peerConnection == null)
			{
				// Queue the candidate for later processing after peer connection is created
				lock (_pendingIceCandidates)
				{
					_pendingIceCandidates.Add(candidateInit);
					_logger?.Debug("Queued remote ICE candidate (peer connection not ready yet)");
				}
				return;
			}

			// Parse candidate string to RTCIceCandidateInit
			// Format: "candidate:..." or full JSON
			var rtcCandidate = new RTCIceCandidateInit
			{
				candidate = candidateInit
			};

			_peerConnection.addIceCandidate(rtcCandidate);
			_logger?.Verbose("✓ Added remote ICE candidate");
		}
		catch (Exception ex)
		{
			_logger?.Warning(ex, "Error adding remote ICE candidate");
		}
	}

	private void SendIceCandidate(RTCIceCandidate candidate)
	{
		try
		{
			if (_wsClient?.IsRunning != true)
				return;

			// Send ICE candidate via protobuf
			var candidateBytes = SimpleProtobufParser.CreateTrickleRequest(
				candidate.ToString(),
				isPublisher: true  // We're publishing audio
			);

			_wsClient.Send(candidateBytes);
			_logger?.Verbose("✓ Sent ICE candidate to server");
		}
		catch (Exception ex)
		{
			_logger?.Warning(ex, "Error sending ICE candidate");
		}
	}

	private void StartPingTimer()
	{
		// Send ping every 10 seconds to keep connection alive
		_pingTimer = new System.Timers.Timer(10000);
		_pingTimer.Elapsed += (sender, e) =>
		{
			try
			{
				if (_wsClient?.IsRunning == true && _isWebRTCConnected)
				{
					var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
					var pingBytes = SimpleProtobufParser.CreatePingRequest(timestamp);
					_wsClient.Send(pingBytes);
					_logger?.Verbose("→ Sent ping (timestamp: {Timestamp})", timestamp);
				}
			}
			catch (Exception ex)
			{
				_logger?.Warning(ex, "Error sending ping");
			}
		};
		_pingTimer.Start();
		_logger?.Information("✓ Started ping timer (10s interval)");
	}

	/// <summary>
	/// Sends audio data over the voice connection.
	/// </summary>
	public Task SendAudioAsync(byte[] opusData, int opusLength)
	{
		if (!_isWebRTCConnected || _peerConnection == null)
		{
			_logger?.Warning("Cannot send audio: WebRTC not connected");
			return Task.CompletedTask;
		}

		// TODO: Implement RTP audio sending
		// This requires creating RTP packets and sending via peer connection
		_logger?.Debug("SendAudioAsync called (RTP streaming not yet implemented)");

		return Task.CompletedTask;
	}

	/// <summary>
	/// Sets speaking state (LiveKit auto-manages speaking indicators).
	/// </summary>
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
			_pingTimer?.Stop();
			_pingTimer?.Dispose();

			_isConnected = false;
			_isWebRTCConnected = false;

			_peerConnection?.close();
			_peerConnection?.Dispose();

			_wsClient?.Dispose();

			_logger?.Information("Disconnected from LiveKit server");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error disconnecting from LiveKit server");
		}

		await Task.CompletedTask;
	}

	public void Dispose()
	{
		DisconnectAsync().Wait();
	}
}
