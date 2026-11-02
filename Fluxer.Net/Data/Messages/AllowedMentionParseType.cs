using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Fluxer.Net;

/// <summary>
/// The kinds of mention a message may ping.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum AllowedMentionParseType
{
    /// <summary>
    /// Users can be mentioned.
    /// </summary>
    [EnumMember(Value = "users")]
    USERS,

    /// <summary>
    /// Roles can be mentioned.
    /// </summary>
    [EnumMember(Value = "roles")]
    ROLES,

    /// <summary>
    /// @everyone and @here can be mentioned.
    /// </summary>
    [EnumMember(Value = "everyone")]
    EVERYONE
}
