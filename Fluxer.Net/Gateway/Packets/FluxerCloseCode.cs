namespace Fluxer.Net.Gateway;

/// <summary>
/// WebSocket close codes specific to the Fluxer Gateway protocol.
/// These codes indicate why a gateway connection was closed.
/// </summary>
public enum FluxerCloseCode
{
    /// <summary>
    /// Unknown error occurred. The connection may be retried.
    /// </summary>
    UnknownError = 4000,

    /// <summary>
    /// An invalid gateway opcode was sent.
    /// </summary>
    UnknownOpcode = 4001,

    /// <summary>
    /// An invalid payload was sent that could not be decoded.
    /// </summary>
    DecodeError = 4002,

    /// <summary>
    /// A payload was sent before authenticating with the gateway.
    /// </summary>
    NotAuthenticated = 4003,

    /// <summary>
    /// The authentication token is invalid.
    /// </summary>
    AuthenticationFailed = 4004,

    /// <summary>
    /// An authentication payload was sent after already authenticating.
    /// </summary>
    AlreadyAuthenticated = 4005,

    /// <summary>
    /// The sequence number sent in a RESUME packet was invalid.
    /// </summary>
    InvalidSequence = 4007,

    /// <summary>
    /// Payloads are being sent too quickly (rate limited).
    /// </summary>
    RateLimited = 4008,

    /// <summary>
    /// The session timed out. A new session should be created.
    /// </summary>
    SessionTimeout = 4009,

    /// <summary>
    /// An invalid shard was specified when connecting.
    /// </summary>
    InvalidShard = 4010,

    /// <summary>
    /// The session requires sharding for this bot to connect.
    /// </summary>
    ShardingRequired = 4011,

    /// <summary>
    /// An invalid API version was specified.
    /// </summary>
    InvalidApiVersion = 4012,

    /// <summary>
    /// Invalid gateway intents were specified.
    /// </summary>
    InvalidIntents = 4013,

    /// <summary>
    /// Disallowed gateway intents were specified (requires verification/approval).
    /// </summary>
    DisallowedIntents = 4014
}
