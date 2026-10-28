using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

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
    [EnumMember(Value = "users")]
    USERS,

    /// <summary>
    /// Allow all role mentions.
    /// </summary>
    [EnumMember(Value = "roles")]
    ROLES,

    /// <summary>
    /// Allow everyone and here mentions.
    /// </summary>
    [EnumMember(Value = "everyone")]
    EVERYONE
}
