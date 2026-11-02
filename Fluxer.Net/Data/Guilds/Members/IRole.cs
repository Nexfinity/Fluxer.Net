namespace Fluxer.Net;

/// <summary>
/// A role in a guild.
/// </summary>
public interface IRole : ISnowflake, IMentionable
{
    /// <summary>
    /// The name of the role.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The permissions granted by the role.
    /// </summary>
    GuildPermissions Permissions { get; }

    /// <summary>
    /// The position of the role in the role hierarchy.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// The hoisted position of the role, or null when <see cref="Position"/> is used instead.
    /// </summary>
    int? HoistPosition { get; }

    /// <summary>
    /// The colour of the role as an integer.
    /// </summary>
    int Color { get; }

    /// <summary>
    /// The hash of the role icon image.
    /// </summary>
    string? IconHash { get; }

    /// <summary>
    /// The unicode emoji shown with the role, or null when it has none.
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
