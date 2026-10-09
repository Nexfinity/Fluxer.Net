namespace Fluxer.Net;

public interface IThreadMember
{
    ulong Id { get; }

    ulong ThreadId { get; }

    DateTimeOffset JoinedAt { get; }

    int Flags { get; }

    IGuildMember? Member { get; }
}
