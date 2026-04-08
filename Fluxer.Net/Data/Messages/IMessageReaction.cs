namespace Fluxer.Net;

public interface IMessageReaction
{
    IEmoji Emoji { get; }

    int Count { get; }

    bool? Me { get; }
}
