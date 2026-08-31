namespace Fluxer.Net;

public interface IVoiceState
{
    /// <summary>
    /// Voice connection session.
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// User id that is connected.
    /// </summary>
    ulong UserId { get; }

    /// <summary>
    /// Channel id connected to.
    /// </summary>
    ulong? ChannelId { get; }

    /// <summary>
    /// Voice connection id.
    /// </summary>
    string? ConnectionId { get; }

    /// <summary>
    /// User is deafened in the guild.
    /// </summary>
    bool IsDeaf { get; }

    /// <summary>
    /// User is muted in the guild.
    /// </summary>
    bool IsMute { get; }

    /// <summary>
    /// Guild id for the voice channel.
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    /// User connected with mobile.
    /// </summary>
    bool IsMobile { get; }

    /// <summary>
    /// User is self deafend.
    /// </summary>
    bool IsSelfDeaf { get; }

    /// <summary>
    /// User is self muted.
    /// </summary>
    bool IsSelfMute { get; }

    /// <summary>
    /// User is streaming.
    /// </summary>
    bool IsSelfStream { get; }

    /// <summary>
    /// User is using video.
    /// </summary>
    bool IsSelfVideo { get; }

    /// <summary>
    /// User can't speak in voice.
    /// </summary>
    bool IsSuppressed { get; }

    /// <summary>
    /// Keys used for streaming.
    /// </summary>
    string[] ViewerStreamKeys { get; }

    /// <summary>
    /// Member for the voice connection.
    /// </summary>
    IGuildMember Member { get; }
}
