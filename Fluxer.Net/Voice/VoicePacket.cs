using Newtonsoft.Json;

namespace Fluxer.Net.Voice;

/// <summary>
/// Represents a packet sent or received over the voice WebSocket connection.
/// </summary>
public class VoicePacket
{
	[JsonProperty("op")]
	public VoiceOpCode OpCode { get; set; }

	[JsonProperty("d")]
	public object? Data { get; set; }
}

/// <summary>
/// Voice Ready payload data.
/// </summary>
public class VoiceReadyPayload
{
	[JsonProperty("ssrc")]
	public uint SSRC { get; set; }

	[JsonProperty("ip")]
	public string Ip { get; set; } = string.Empty;

	[JsonProperty("port")]
	public int Port { get; set; }

	[JsonProperty("modes")]
	public string[] Modes { get; set; } = Array.Empty<string>();

	[JsonProperty("heartbeat_interval")]
	public int HeartbeatInterval { get; set; }
}

/// <summary>
/// Voice Session Description payload data.
/// </summary>
public class VoiceSessionDescriptionPayload
{
	[JsonProperty("mode")]
	public string Mode { get; set; } = string.Empty;

	[JsonProperty("secret_key")]
	public byte[] SecretKey { get; set; } = Array.Empty<byte>();
}

/// <summary>
/// Voice Hello payload data.
/// </summary>
public class VoiceHelloPayload
{
	[JsonProperty("heartbeat_interval")]
	public double HeartbeatInterval { get; set; }
}

/// <summary>
/// Voice Identify payload data.
/// </summary>
public class VoiceIdentifyPayload
{
	[JsonProperty("server_id")]
	public ulong ServerId { get; set; }

	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("session_id")]
	public string SessionId { get; set; } = string.Empty;

	[JsonProperty("token")]
	public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Voice Select Protocol payload data.
/// </summary>
public class VoiceSelectProtocolPayload
{
	[JsonProperty("protocol")]
	public string Protocol { get; set; } = "udp";

	[JsonProperty("data")]
	public VoiceProtocolData Data { get; set; } = new();
}

/// <summary>
/// Voice protocol data for Select Protocol.
/// </summary>
public class VoiceProtocolData
{
	[JsonProperty("address")]
	public string Address { get; set; } = string.Empty;

	[JsonProperty("port")]
	public ushort Port { get; set; }

	[JsonProperty("mode")]
	public string Mode { get; set; } = string.Empty;
}

/// <summary>
/// Voice Speaking payload data.
/// </summary>
public class VoiceSpeakingPayload
{
	[JsonProperty("speaking")]
	public int Speaking { get; set; }

	[JsonProperty("delay")]
	public int Delay { get; set; }

	[JsonProperty("ssrc")]
	public uint SSRC { get; set; }
}
