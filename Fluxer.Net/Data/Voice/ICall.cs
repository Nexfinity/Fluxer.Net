namespace Fluxer.Net;

public interface ICall
{
    ulong ChannelId { get; }

    ulong MessageId { get; }

    string? Region { get; }

    ulong[] RingingUsers { get; }

    IVoiceState[] VoiceStates { get; }

    ulong[]? Recipients { get; }

    DateTimeOffset? CreatedAt { get; }
}
