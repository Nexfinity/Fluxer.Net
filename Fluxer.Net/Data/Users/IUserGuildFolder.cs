namespace Fluxer.Net;

/// <summary>
/// A folder that groups guilds in the user's sidebar.
/// </summary>
public interface IUserGuildFolder
{
    /// <summary>
    /// The id of the folder.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The name of the folder, or null when it has none.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// The color of the folder, or null when it has none.
    /// </summary>
    int? Color { get; }

    /// <summary>
    /// The ids of the guilds in the folder, or null when there are none.
    /// </summary>
    List<ulong>? GuildIds { get; }
}
