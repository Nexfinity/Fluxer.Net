using Fluxer.Net.Extensions;
namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <summary>
/// The places a search can cover. Serialized by <see cref="SearchScopeConverter"/>.
/// </summary>
public enum SearchScope
{
    /// <summary>
    /// The current channel or guild.
    /// </summary>
    Current,

    /// <summary>
    /// The open direct messages.
    /// </summary>
    OpenDMs,

    /// <summary>
    /// All direct messages.
    /// </summary>
    AllDMs,

    /// <summary>
    /// All guilds.
    /// </summary>
    AllGuilds,

    /// <summary>
    /// All direct messages and guilds.
    /// </summary>
    All,

    /// <summary>
    /// The open direct messages and all guilds.
    /// </summary>
    OpenDMsAndAllGuilds
}
