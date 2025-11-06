using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Websocket.Client;

namespace Fluxer.Net.Voice;

/// <summary>
/// Manages voice WebSocket and UDP connections for real-time audio streaming.
/// Based on Discord's voice protocol implementation.
/// </summary>
public class VoiceClient : IDisposable
{
	private readonly string _endpoint;
	private readonly ulong _guildId;
	private readonly ulong _userId;
	private readonly string _sessionId;
	private readonly string _token;
	private readonly ILogger? _logger;

	private WebsocketClient? _wsClient;
	private UdpClient? _udpClient;
	private IPEndPoint? _udpEndpoint;
	private RTPPacket? _rtpHandler;
	private uint _ssrc;
	private byte[]? _secretKey;
	private string _encryptionMode = "xsalsa20_poly1305";
	private uint _nonceCounter;

	private Task? _heartbeatTask;
	private CancellationTokenSource? _heartbeatCts;
	private bool _isConnected;

	public event Action? OnReady;
	public event Action<Exception>? OnError;

	/// <summary>
	/// Creates a new VoiceClient instance.
	/// </summary>
	/// <param name="endpoint">Voice server endpoint (from VOICE_SERVER_UPDATE event).</param>
	/// <param name="guildId">Guild/server ID.</param>
	/// <param name="userId">Bot's user ID.</param>
	/// <param name="sessionId">Session ID (from VOICE_STATE_UPDATE event).</param>
	/// <param name="token">Voice token (from VOICE_SERVER_UPDATE event).</param>
	/// <param name="logger">Optional Serilog logger.</param>
	public VoiceClient(string endpoint, ulong guildId, ulong userId, string sessionId, string token, ILogger? logger = null)
	{
		_endpoint = endpoint.Replace(":80", ""); // Remove port if present
		_guildId = guildId;
		_userId = userId;
		_sessionId = sessionId;
		_token = token;
		_logger = logger;
	}

	/// <summary>
	/// Connects to the voice server and establishes both WebSocket and UDP connections.
	/// </summary>
	public async Task ConnectAsync()
	{
		try
		{
			_logger?.Information("Connecting to voice server: {Endpoint}", _endpoint);

			// Connect to voice WebSocket
			var wsUrl = new Uri($"wss://{_endpoint}/?v=4");
			_wsClient = new WebsocketClient(wsUrl);
			_wsClient.MessageReceived.Subscribe(HandleWebSocketMessage);

			await _wsClient.Start();
			_logger?.Information("Voice WebSocket connected");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Failed to connect to voice server");
			OnError?.Invoke(ex);
			throw;
		}
	}

	private void HandleWebSocketMessage(ResponseMessage message)
	{
		try
		{
			var packet = JsonConvert.DeserializeObject<VoicePacket>(message.Text);
			if (packet == null) return;

			_logger?.Debug("Voice OpCode received: {OpCode}", packet.OpCode);

			switch (packet.OpCode)
			{
				case VoiceOpCode.Hello:
					HandleHello(packet.Data);
					break;

				case VoiceOpCode.Ready:
					HandleReady(packet.Data);
					break;

				case VoiceOpCode.SessionDescription:
					HandleSessionDescription(packet.Data);
					break;

				case VoiceOpCode.HeartbeatAck:
					_logger?.Debug("Voice heartbeat acknowledged");
					break;

				case VoiceOpCode.Speaking:
					// Handle speaking updates (optional)
					break;

				default:
					_logger?.Warning("Unhandled voice OpCode: {OpCode}", packet.OpCode);
					break;
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling voice WebSocket message");
			OnError?.Invoke(ex);
		}
	}

	private void HandleHello(object? data)
	{
		try
		{
			var json = JObject.FromObject(data ?? new object());
			var helloPayload = json.ToObject<VoiceHelloPayload>();

			if (helloPayload != null)
			{
				_logger?.Information("Voice Hello received. Heartbeat interval: {Interval}ms", helloPayload.HeartbeatInterval);

				// Start heartbeat
				StartHeartbeat(helloPayload.HeartbeatInterval);

				// Send Identify
				SendIdentify();
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling Voice Hello");
		}
	}

	private void HandleReady(object? data)
	{
		try
		{
			var json = JObject.FromObject(data ?? new object());
			var readyPayload = json.ToObject<VoiceReadyPayload>();

			if (readyPayload != null)
			{
				_ssrc = readyPayload.SSRC;
				_logger?.Information("Voice Ready received. SSRC: {SSRC}, IP: {IP}, Port: {Port}",
					readyPayload.SSRC, readyPayload.Ip, readyPayload.Port);

				// Establish UDP connection and perform IP discovery
				EstablishUdpConnection(readyPayload.Ip, readyPayload.Port).Wait();
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling Voice Ready");
		}
	}

	private void HandleSessionDescription(object? data)
	{
		try
		{
			var json = JObject.FromObject(data ?? new object());
			var sessionPayload = json.ToObject<VoiceSessionDescriptionPayload>();

			if (sessionPayload != null)
			{
				_secretKey = sessionPayload.SecretKey;
				_encryptionMode = sessionPayload.Mode;
				_logger?.Information("Session description received. Encryption mode: {Mode}", sessionPayload.Mode);

				_isConnected = true;
				OnReady?.Invoke();
			}
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error handling Session Description");
		}
	}

	private async Task EstablishUdpConnection(string ip, int port)
	{
		try
		{
			_udpClient = new UdpClient();
			_udpEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
			_udpClient.Connect(_udpEndpoint);

			_logger?.Information("UDP connection established to {IP}:{Port}", ip, port);

			// Perform IP discovery
			var (localIp, localPort) = await PerformIpDiscovery();

			// Send Select Protocol
			SendSelectProtocol(localIp, localPort);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error establishing UDP connection");
			throw;
		}
	}

	private async Task<(string, ushort)> PerformIpDiscovery()
	{
		try
		{
			// Create IP discovery packet (70 bytes)
			byte[] discoveryPacket = new byte[70];
			BitConverter.GetBytes(_ssrc).CopyTo(discoveryPacket, 0);

			// Send discovery packet
			await _udpClient!.SendAsync(discoveryPacket, discoveryPacket.Length);

			// Receive response
			var response = await _udpClient.ReceiveAsync();
			byte[] data = response.Buffer;

			// Extract IP (null-terminated string starting at byte 4)
			int ipStart = 4;
			int ipEnd = Array.IndexOf(data, (byte)0, ipStart);
			string localIp = Encoding.UTF8.GetString(data, ipStart, ipEnd - ipStart);

			// Extract port (last 2 bytes)
			ushort localPort = BitConverter.ToUInt16(data, data.Length - 2);

			_logger?.Information("IP Discovery complete. Local IP: {IP}, Port: {Port}", localIp, localPort);

			return (localIp, localPort);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error performing IP discovery");
			throw;
		}
	}

	private void SendIdentify()
	{
		var identify = new VoicePacket
		{
			OpCode = VoiceOpCode.Identify,
			Data = new VoiceIdentifyPayload
			{
				ServerId = _guildId,
				UserId = _userId,
				SessionId = _sessionId,
				Token = _token
			}
		};

		Send(identify);
		_logger?.Debug("Sent Voice Identify");
	}

	private void SendSelectProtocol(string address, ushort port)
	{
		var selectProtocol = new VoicePacket
		{
			OpCode = VoiceOpCode.SelectProtocol,
			Data = new VoiceSelectProtocolPayload
			{
				Protocol = "udp",
				Data = new VoiceProtocolData
				{
					Address = address,
					Port = port,
					Mode = "xsalsa20_poly1305"
				}
			}
		};

		Send(selectProtocol);
		_logger?.Debug("Sent Select Protocol");
	}

	private void Send(VoicePacket packet)
	{
		if (_wsClient == null || !_wsClient.IsRunning) return;

		var json = JsonConvert.SerializeObject(packet);
		_wsClient.Send(json);
	}

	private void StartHeartbeat(double intervalMs)
	{
		_heartbeatCts = new CancellationTokenSource();
		_heartbeatTask = Task.Run(async () =>
		{
			while (!_heartbeatCts.Token.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(TimeSpan.FromMilliseconds(intervalMs), _heartbeatCts.Token);

					var heartbeat = new VoicePacket
					{
						OpCode = VoiceOpCode.Heartbeat,
						Data = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
					};

					Send(heartbeat);
					_logger?.Debug("Voice heartbeat sent");
				}
				catch (TaskCanceledException)
				{
					break;
				}
				catch (Exception ex)
				{
					_logger?.Error(ex, "Error sending voice heartbeat");
				}
			}
		}, _heartbeatCts.Token);
	}

	/// <summary>
	/// Sends Opus-encoded audio data over the voice connection.
	/// </summary>
	/// <param name="opusData">Opus-encoded audio frame.</param>
	/// <param name="opusLength">Length of Opus data.</param>
	public async Task SendAudioAsync(byte[] opusData, int opusLength)
	{
		if (!_isConnected || _udpClient == null || _udpEndpoint == null || _secretKey == null)
		{
			_logger?.Warning("Cannot send audio: not connected or missing required data");
			return;
		}

		try
		{
			// Initialize RTP handler if needed
			_rtpHandler ??= new RTPPacket(_ssrc);

			// Create RTP packet
			byte[] rtpPacket = _rtpHandler.CreatePacket(opusData, opusLength);

			// Encrypt packet
			byte[] encryptedPacket = _encryptionMode switch
			{
				"xsalsa20_poly1305" => AudioEncryption.Encrypt(rtpPacket, _secretKey),
				"xsalsa20_poly1305_suffix" => AudioEncryption.EncryptWithSuffix(rtpPacket, _secretKey),
				"xsalsa20_poly1305_lite" => AudioEncryption.EncryptWithLite(rtpPacket, _secretKey, ref _nonceCounter),
				_ => throw new NotSupportedException($"Encryption mode '{_encryptionMode}' is not supported")
			};

			// Send over UDP
			await _udpClient.SendAsync(encryptedPacket, encryptedPacket.Length);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error sending audio");
			OnError?.Invoke(ex);
		}
	}

	/// <summary>
	/// Sets the speaking state.
	/// </summary>
	/// <param name="speaking">True if speaking, false if not speaking.</param>
	public void SetSpeaking(bool speaking)
	{
		var speakingPacket = new VoicePacket
		{
			OpCode = VoiceOpCode.Speaking,
			Data = new VoiceSpeakingPayload
			{
				Speaking = speaking ? 1 : 0,
				Delay = 0,
				SSRC = _ssrc
			}
		};

		Send(speakingPacket);
		_logger?.Debug("Speaking state set to: {Speaking}", speaking);
	}

	/// <summary>
	/// Disconnects from the voice server.
	/// </summary>
	public async Task DisconnectAsync()
	{
		try
		{
			_isConnected = false;

			_heartbeatCts?.Cancel();
			if (_heartbeatTask != null)
				await _heartbeatTask;

			_wsClient?.Dispose();
			_udpClient?.Dispose();

			_logger?.Information("Disconnected from voice server");
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error disconnecting from voice server");
		}
	}

	public void Dispose()
	{
		DisconnectAsync().Wait();
		_heartbeatCts?.Dispose();
	}
}
