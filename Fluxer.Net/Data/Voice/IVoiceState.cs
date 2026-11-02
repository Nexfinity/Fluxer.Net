namespace Fluxer.Net;

/// <summary>
/// A user's presence in a voice channel.
/// </summary>
public interface IVoiceState
{
    /// <summary>
    /// The voice session id of the state.
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// The id of the user in voice.
    /// </summary>
    ulong UserId { get; }

    /// <summary>
    /// The id of the voice channel the user is in, or null when the user is not in a channel.
    /// </summary>
    ulong? ChannelId { get; }

    /// <summary>
    /// The id of the voice connection.
    /// </summary>
    string? ConnectionId { get; }

    /// <summary>
    /// Whether the user is deafened in the guild.
    /// </summary>
    bool IsDeaf { get; }

    /// <summary>
    /// Whether the user is muted in the guild.
    /// </summary>
    bool IsMute { get; }

    /// <summary>
    /// The id of the guild of the voice channel.
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    /// Whether the user is connected from a mobile client.
    /// </summary>
    bool IsMobile { get; }

    /// <summary>
    /// Whether the user has deafened themselves.
    /// </summary>
    bool IsSelfDeaf { get; }

    /// <summary>
    /// Whether the user has muted themselves.
    /// </summary>
    bool IsSelfMute { get; }

    /// <summary>
    /// Whether the user is streaming.
    /// </summary>
    bool IsSelfStream { get; }

    /// <summary>
    /// Whether the user has video turned on.
    /// </summary>
    bool IsSelfVideo { get; }

    /// <summary>
    /// Whether the user is suppressed and cannot speak in voice.
    /// </summary>
    bool IsSuppressed { get; }

    /// <summary>
    /// The keys of the streams the user is watching.
    /// </summary>
    string[] ViewerStreamKeys { get; }

    /// <summary>
    /// The guild member of the user, or null when it is not sent.
    /// </summary>
    IGuildMember? Member { get; }
}
