namespace Fluxer.Net;

public interface IGuildSticker : ISticker
{
    string? Description { get; }

    List<string>? Tags { get; }

    User? Creator { get; }
}
