namespace Fluxer.Net;

public interface IGuildSticker
{
    ulong Id { get; }

    string Name { get; }

    string? Description { get; }

    List<string>? Tags { get; }

    bool IsAnimated { get; }

    User? Creator { get; }
}
