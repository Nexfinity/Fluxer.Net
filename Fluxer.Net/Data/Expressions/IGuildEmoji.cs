namespace Fluxer.Net;

public interface IGuildEmoji : IEmoji
{
    User? Creator { get; }
}
