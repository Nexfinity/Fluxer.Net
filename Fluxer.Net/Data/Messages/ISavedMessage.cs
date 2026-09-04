namespace Fluxer.Net;

public interface ISavedMessage : ISnowflake
{
    ulong UserId { get; }

    ulong ChannelId { get; }

    DateTimeOffset SavedAt { get; }
}
