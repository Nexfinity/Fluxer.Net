using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Fluxer.Net;

/// <summary>
/// Which mentions should be used when sending a message.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum AllowedMentionParseType
{
    /// <summary>
    /// Allow all user mentions.
    /// </summary>
    USERS,

    /// <summary>
    /// Allow all role mentions.
    /// </summary>
    ROLES,

    /// <summary>
    /// Allow everyone and here mentions.
    /// </summary>
    EVERYONE
}
