using Fluxer.Net.Extensions;
namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <see cref="SearchScopeConverter"/>
public enum SearchScope
{
    Current,
    OpenDMs,
    AllDMs,
    AllGuilds,
    All,
    OpenDMsAndAllGuilds
}
