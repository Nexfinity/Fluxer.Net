namespace Fluxer.Net.Voice;

/// <summary>
/// Operation codes used in the Voice Gateway WebSocket protocol.
/// Based on Discord's voice protocol specifications.
/// </summary>
public enum VoiceOpCode
{
	/// <summary>
	/// Client → Server: Begin a voice websocket connection.
	/// </summary>
	Identify = 0,

	/// <summary>
	/// Client → Server: Select the voice protocol.
	/// </summary>
	SelectProtocol = 1,

	/// <summary>
	/// Server → Client: Complete the websocket handshake.
	/// Contains SSRC, UDP port, and encryption modes.
	/// </summary>
	Ready = 2,

	/// <summary>
	/// Client → Server: Keep the websocket connection alive.
	/// </summary>
	Heartbeat = 3,

	/// <summary>
	/// Server → Client: Describe the session.
	/// Contains secret key for encryption.
	/// </summary>
	SessionDescription = 4,

	/// <summary>
	/// Client/Server: Indicate which users are speaking.
	/// </summary>
	Speaking = 5,

	/// <summary>
	/// Server → Client: Sent to acknowledge a received client heartbeat.
	/// </summary>
	HeartbeatAck = 6,

	/// <summary>
	/// Client → Server: Resume a connection.
	/// </summary>
	Resume = 7,

	/// <summary>
	/// Server → Client: Sent after a successful Resume.
	/// </summary>
	Hello = 8,

	/// <summary>
	/// Server → Client: Sent after a successful Resume.
	/// </summary>
	Resumed = 9,

	/// <summary>
	/// Client → Server: Request member resource.
	/// </summary>
	ClientDisconnect = 13
}
