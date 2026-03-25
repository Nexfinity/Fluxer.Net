namespace Fluxer.Net;

public interface IGuildEmoji
{
    ulong Id { get; }

    string Name { get; }

    bool IsAnimated { get; }

    User? Creator { get; }
}
