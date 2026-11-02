namespace Fluxer.Net;

/// <summary>
/// The status and activities of a user.
/// </summary>
public interface IPresence
{
    /// <summary>
    /// The id of the user.
    /// </summary>
    public ulong UserId { get; }

    /// <summary>
    /// The id of the guild the presence belongs to, or null when it has none.
    /// </summary>
    public ulong? GuildId { get; }

    /// <summary>
    /// The status of the user, as the API sends it.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// The activities the user is doing, or null when there are none.
    /// </summary>
    public IEnumerable<IActivity>? Activities { get; }

    /// <summary>
    /// The status of the user on each client platform, or null when there is none.
    /// </summary>
    public IClientStatus? ClientStatus { get; }
}
