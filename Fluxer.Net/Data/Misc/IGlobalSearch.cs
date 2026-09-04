namespace Fluxer.Net;

public interface IGlobalSearch
{
    IMessage[] Messages { get; }

    IChannel[] Channels { get; }

    ulong Total { get; }

    int HitsPerPage { get; }

    int Page { get; }
}
