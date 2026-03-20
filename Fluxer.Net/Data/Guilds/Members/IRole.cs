namespace Fluxer.Net;

public interface IRole
{
    ulong GuildId { get; }

    ulong Id { get; }

    string Name { get; }

    ulong Permissions { get; }

    int Position { get; }

    int Color { get; }

    string? IconHash { get; }

    string? UnicodeEmoji { get; }

    bool IsHoisted { get; }

    bool IsMentionable { get; }
}
