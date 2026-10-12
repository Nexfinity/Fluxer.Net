namespace Fluxer.Net;

public interface IReaction
{
    ulong UserId { get; }
    ulong MessageId { get; }
    IEmoji Emoji { get; }
}
