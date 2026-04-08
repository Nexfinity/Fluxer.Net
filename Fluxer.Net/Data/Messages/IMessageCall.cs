namespace Fluxer.Net;

public interface IMessageCall
{
    HashSet<ulong> Participants { get; }

    DateTime? EndedAt { get; }
}
