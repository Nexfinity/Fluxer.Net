namespace Fluxer.Net;

/// <summary>
/// The voice server Fluxer assigns to a voice connection.
/// </summary>
public interface IVoiceServer
{
    /// <summary>
    /// The access token for the voice server room.
    /// </summary>
    string Token { get; }

    /// <summary>
    /// The address of the voice server.
    /// </summary>
    string Endpoint { get; }

    /// <summary>
    /// The id of the voice connection.
    /// </summary>
    string ConnectionId { get; }

    /// <summary>
    /// The voice channel the connection is for.
    /// </summary>
    ulong ChannelId { get; }

    /// <summary>
    /// The guild of the voice channel, or null when it is not sent.
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    /// The shared key for end-to-end encryption of the audio, or null when the audio is not encrypted.
    /// </summary>
    string? E2EEKey { get; }
}
