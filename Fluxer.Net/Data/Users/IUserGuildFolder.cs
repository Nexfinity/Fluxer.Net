namespace Fluxer.Net;

public interface IUserGuildFolder
{
    int Id { get; }

    string? Name { get; }

    int? Color { get; }

    List<ulong>? GuildIds { get; }
}
