namespace Fluxer.Net;

public interface IVoiceState
{
    string SessionId { get; }

    bool UserId { get; }

    ulong? ChannelId { get; }

    string? ConnectionId { get; }

    bool IsDeaf { get; }

    bool IsMute { get; }

    ulong? GuildId { get; }

    bool IsMobile { get; }

    bool IsSelfDeaf { get; }

    bool IsSelfMute { get; }

    bool IsSelfStream { get; }

    bool IsSelfVideo { get; }

    string[] ViewerStreamKeys { get; }

    IGuildMember Member { get; }
}
