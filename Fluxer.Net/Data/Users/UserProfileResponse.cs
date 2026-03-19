using Newtonsoft.Json;

namespace Fluxer.Net;

public class UserProfileResponse : Entity
{
    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("user_profile")]
    public UserProfile Profile { get; set; }

    [JsonProperty("guild_member")]
    public GuildMember? Member { get; set; }

    [JsonProperty("guild_member_profile")]
    public UserProfile? MemberProfile { get; set; }

    [JsonProperty("premium_type")]
    public int PremiumType { get; set; }

    [JsonProperty("premium_since")]
    public DateTime? PremiumSince { get; set; }

    [JsonProperty("mutual_friends")]
    public User[]? MutualFriends { get; set; }

    [JsonProperty("mutual_guilds")]
    public Guild[]? MutualGuilds { get; set; }

    [JsonProperty("connected_accounts")]
    public UserConnection[] Connections { get; set; }
}
