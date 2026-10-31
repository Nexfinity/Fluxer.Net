using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Sets the nickname you give a friend.
/// </summary>
public class UpdateRelationshipNicknameRequest
{
    /// <summary>
    /// The nickname, up to 256 characters, or null to clear it.
    /// </summary>
    [JsonProperty("nickname", NullValueHandling = NullValueHandling.Include)]
    public string? Nickname { get; set; }
}
