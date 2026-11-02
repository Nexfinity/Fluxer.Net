using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The response of a request for a user's profile.
/// </summary>
public class UserProfileResponse
{
    /// <summary>
    /// The user the profile belongs to.
    /// </summary>
    [JsonProperty("user")]
    public UserJson User { get; set; }

    /// <summary>
    /// The profile of the user.
    /// </summary>
    [JsonProperty("user_profile")]
    public UserProfileJson Profile { get; set; }

    /// <summary>
    /// The guild member of the user, or null when there is none.
    /// </summary>
    [JsonProperty("guild_member")]
    public GuildMemberJson? Member { get; set; }

    /// <summary>
    /// The profile of the user in the guild, or null when there is none.
    /// </summary>
    [JsonProperty("guild_member_profile")]
    public UserProfileJson? MemberProfile { get; set; }

    /// <summary>
    /// The premium type of the user, as the API sends it.
    /// </summary>
    [JsonProperty("premium_type")]
    public int PremiumType { get; set; }

    /// <summary>
    /// When premium was first activated, or null when it never was.
    /// </summary>
    [JsonProperty("premium_since")]
    public DateTimeOffset? PremiumSince { get; set; }

    /// <summary>
    /// The friends the user has in common with the current user, or null when there are none.
    /// </summary>
    [JsonProperty("mutual_friends")]
    public UserJson[]? MutualFriends { get; set; }

    /// <summary>
    /// The guilds the user has in common with the current user, or null when there are none.
    /// </summary>
    [JsonProperty("mutual_guilds")]
    public GuildJson[]? MutualGuilds { get; set; }

    /// <summary>
    /// The connected accounts of the user.
    /// </summary>
    [JsonProperty("connected_accounts")]
    public UserConnectionJson[] Connections { get; set; }
}
