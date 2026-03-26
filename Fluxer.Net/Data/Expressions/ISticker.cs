namespace Fluxer.Net;

public interface ISticker
{
    ulong Id { get; }

    string Name { get; }

    bool IsAnimated { get; }
}
