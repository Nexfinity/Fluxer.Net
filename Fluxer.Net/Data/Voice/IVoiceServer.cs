namespace Fluxer.Net;

public interface IVoiceServer
{
    string Token { get; }

    string Endpoint { get; }

    string ConnectionId { get; }

    ulong ChannelId { get; }

    ulong? GuildId { get; }

    string? E2EEKey { get; }
}
