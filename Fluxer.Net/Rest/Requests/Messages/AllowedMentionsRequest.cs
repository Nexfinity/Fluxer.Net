using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Allowed mentions when sending a message.
/// </summary>
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
    [JsonProperty("users")]
    public HashSet<ulong>? Users { get; set; }

    /// <summary>
    /// Array of role IDs to mention (max 100)
    /// </summary>
    [JsonProperty("roles")]
    public HashSet<ulong>? Roles { get; set; }

    /// <summary>
    /// Whether to mention the author of the replied message
    /// </summary>
    [JsonProperty("replied_user")]
    public bool? RepliedUser { get; set; }

    /// <summary>
    /// Mentions nobody, including the author of a replied-to message.
    /// </summary>
    public static AllowedMentionsRequest None => new AllowedMentionsRequest
    {
        Parse = new HashSet<AllowedMentionParseType>(),
        RepliedUser = false
    };

    /// <summary>
    /// Mentions every user, role and everyone or here mention in the content, and the author of a replied-to message.
    /// </summary>
    public static AllowedMentionsRequest All => new AllowedMentionsRequest
    {
        Parse = new HashSet<AllowedMentionParseType> { AllowedMentionParseType.USERS, AllowedMentionParseType.ROLES, AllowedMentionParseType.EVERYONE },
        RepliedUser = true
    };

    /// <summary>
    /// Mentions only the given users and roles.
    /// </summary>
    public static AllowedMentionsRequest Only(IEnumerable<ulong>? users = null, IEnumerable<ulong>? roles = null) => new AllowedMentionsRequest
    {
        Parse = new HashSet<AllowedMentionParseType>(),
        Users = users == null ? null : new HashSet<ulong>(users),
        Roles = roles == null ? null : new HashSet<ulong>(roles),
        RepliedUser = false
    };
}
