using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Fluxer.Net;

/// <summary>
/// How a forum filters posts when several tags are picked in its tag search.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ForumTagMatchSetting
{
    /// <summary>
    /// Show posts with any of the picked tags.
    /// </summary>
    [EnumMember(Value = "match_some")]
    MatchSome,

    /// <summary>
    /// Show only posts with all of the picked tags.
    /// </summary>
    [EnumMember(Value = "match_all")]
    MatchAll
}
