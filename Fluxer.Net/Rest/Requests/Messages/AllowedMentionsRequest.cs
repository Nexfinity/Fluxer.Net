using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

public class AllowedMentionsRequest
{
    /// <summary>
    /// Types of mentions to parse from content
    /// </summary>
    [JsonProperty("parse")]
    public HashSet<AllowedMentionParseType>? Parse { get; set; }

    /// <summary>
    /// Array of user IDs to mention (max 100)
    /// </summary>
    [MaxLength(100)]
    [JsonProperty("users")]
    public HashSet<ulong>? Users { get; set; }

    /// <summary>
    /// Array of role IDs to mention (max 100)
    /// </summary>
    [MaxLength(100)]
    [JsonProperty("roles")]
    public HashSet<ulong>? Roles { get; set; }

    /// <summary>
    /// Whether to mention the author of the replied message
    /// </summary>
    [JsonProperty("replied_user")]
    public bool? RepliedUser { get; set; }
}
