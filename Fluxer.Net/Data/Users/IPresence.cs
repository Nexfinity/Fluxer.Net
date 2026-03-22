namespace Fluxer.Net;

public interface IPresence
{
    public ulong UserId { get; }

    public ulong? GuildId { get; }

    public string Status { get; }

    public IEnumerable<IActivity>? Activities { get; }

    public IClientStatus? ClientStatus { get; }
}
