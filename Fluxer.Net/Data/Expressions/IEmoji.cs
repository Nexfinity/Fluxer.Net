namespace Fluxer.Net;

public interface IEmoji
{
    ulong Id { get; }

    string Name { get; }

    bool IsAnimated { get; }
}
