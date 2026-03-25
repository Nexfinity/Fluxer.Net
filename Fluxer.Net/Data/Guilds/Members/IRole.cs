namespace Fluxer.Net;

public interface IRole
{
    /// <summary>
    /// The unique identifier for this role.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The name of the role.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The role's permission bitfield. Sent as a quoted string by the gateway (e.g. "8933636165184").
    /// </summary>
    ulong Permissions { get; }

    /// <summary>
    /// The position of the role in the role hierarchy.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// The colour of the role as an integer.
    /// </summary>
    int Color { get; }

    /// <summary>
    /// The unicode emoji for this role.
    /// </summary>
    string? UnicodeEmoji { get; }

    /// <summary>
    /// Whether this role is displayed separately in the member list.
    /// </summary>
    bool IsHoisted { get; }

    /// <summary>
    /// Whether this role can be mentioned by anyone.
    /// </summary>
    bool IsMentionable { get; }
}
