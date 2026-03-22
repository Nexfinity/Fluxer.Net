namespace Fluxer.Net.Gateway.Packets;

/// <summary>
/// Operation codes used in the Fluxer Gateway WebSocket protocol.
/// These codes determine how gateway packets should be processed.
/// </summary>
/// <remarks>
/// OpCodes are used in <see cref="GatewayPacket.OpCode"/> to identify the type of message
/// being sent or received over the gateway WebSocket connection. The gateway uses these
/// codes to route packets to appropriate handlers for events, heartbeats, authentication, and more.
/// </remarks>
public enum FluxerOpCode
{
    /// <summary>
    /// Dispatches an event from the server to the client.
    /// Server → Client. Contains event data in the data field.
    /// </summary>
    /// <remarks>
    /// All gateway events (MESSAGE_CREATE, GUILD_UPDATE, etc.) use this opcode.
    /// The specific event type is identified by the <see cref="GatewayPacket.Dispatch"/> field.
    /// </remarks>
    Dispatch = 0,

    /// <summary>
    /// Heartbeat packet to maintain connection.
    /// Bidirectional. Client sends periodically, server may request immediate heartbeat.
    /// </summary>
    /// <remarks>
    /// Clients must send heartbeat packets at the interval specified in the HELLO event.
    /// The server responds with <see cref="HeartbeatAck"/>. Failure to heartbeat will result in disconnection.
    /// </remarks>
    Heartbeat = 1,

    /// <summary>
    /// Identifies a new session and authenticates the client.
    /// Client → Server. Sent immediately after connection to authenticate.
    /// </summary>
    /// <remarks>
    /// Contains authentication token, client properties, presence, and event filters.
    /// Must be sent before any other client packets (except RESUME).
    /// See <see cref="Data.IdentifyGatewayData"/> for payload structure.
    /// </remarks>
    Identify = 2,

    /// <summary>
    /// Updates the client's presence (online status, activity).
    /// Client → Server. Changes the user's visible status.
    /// </summary>
    /// <remarks>
    /// Used to change online status (Online, Idle, Do Not Disturb, Invisible).
    /// See <see cref="Data.Users.PresenceUpdateGatewayData"/> for payload structure.
    /// </remarks>
    PresenceUpdate = 3,

    /// <summary>
    /// Updates voice state (join/leave voice channels, mute, deafen).
    /// Client → Server. Controls voice channel participation.
    /// </summary>
    /// <remarks>
    /// Used to join or leave voice channels and update voice settings.
    /// Server responds with VOICE_STATE_UPDATE and VOICE_SERVER_UPDATE events.
    /// </remarks>
    VoiceStateUpdate = 4,

    /// <summary>
    /// Pings the voice server to maintain voice connection.
    /// Client → Server. Used during active voice connections.
    /// </summary>
    VoiceServerPing = 5,

    /// <summary>
    /// Resumes a previous session after disconnection.
    /// Client → Server. Attempts to replay missed events from last sequence.
    /// </summary>
    /// <remarks>
    /// Sent instead of IDENTIFY when reconnecting with a valid session ID.
    /// Contains session ID, sequence number, and token.
    /// Server responds with RESUMED event if successful, or INVALID_SESSION if failed.
    /// See <see cref="Data.ReconnectGatewayData"/> for payload structure.
    /// </remarks>
    Resume = 6,

    /// <summary>
    /// Server requests the client to reconnect.
    /// Server → Client. Client should immediately reconnect and attempt RESUME.
    /// </summary>
    /// <remarks>
    /// Server is requesting a controlled reconnection (e.g., for load balancing).
    /// Client should disconnect and reconnect, attempting to RESUME the session.
    /// </remarks>
    Reconnect = 7,

    /// <summary>
    /// Requests guild member list for a specific guild.
    /// Client → Server. Used to fetch member information for large guilds.
    /// </summary>
    /// <remarks>
    /// For guilds with many members, not all members are sent in GUILD_CREATE.
    /// This opcode requests additional member data. Server responds with GUILD_MEMBERS_CHUNK events.
    /// </remarks>
    RequestGuildMembers = 8,

    /// <summary>
    /// Indicates the session is invalid and cannot be resumed.
    /// Server → Client. Session must be terminated and new IDENTIFY sent.
    /// </summary>
    /// <remarks>
    /// Contains a boolean indicating if the session is resumable.
    /// If false, client must clear session state and send new IDENTIFY.
    /// If true, client may attempt RESUME again.
    /// </remarks>
    InvalidSession = 9,

    /// <summary>
    /// Initial message sent by server after connection.
    /// Server → Client. Contains heartbeat interval.
    /// </summary>
    /// <remarks>
    /// First message received after WebSocket connection is established.
    /// Contains the heartbeat interval in milliseconds that the client must use.
    /// Client should begin sending heartbeats and then send IDENTIFY or RESUME.
    /// See <see cref="Data.HelloGatewayData"/> for payload structure.
    /// </remarks>
    Hello = 10,

    /// <summary>
    /// Acknowledges receipt of a heartbeat.
    /// Server → Client. Confirms heartbeat was received.
    /// </summary>
    /// <remarks>
    /// Server sends this in response to client HEARTBEAT packets.
    /// If client does not receive ACK, the connection may be unhealthy.
    /// </remarks>
    HeartbeatAck = 11,

    /// <summary>
    /// Connects to a call (voice/video).
    /// Client → Server. Initiates call connection.
    /// </summary>
    CallConnect = 13,

    /// <summary>
    /// Subscribes to events for specific guilds.
    /// Client → Server. Controls which guild events are received.
    /// </summary>
    /// <remarks>
    /// Used to enable or disable events for specific guilds.
    /// Useful for managing event load in bots that are in many guilds.
    /// </remarks>
    GuildSubscriptions = 14
}
