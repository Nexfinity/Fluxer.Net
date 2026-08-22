using Newtonsoft.Json;

namespace Fluxer.Net;

public class UserProfileResponse
{
    [JsonProperty("user")]
    public UserJson User { get; set; }

    [JsonProperty("user_profile")]
    public UserProfileJson Profile { get; set; }

    [JsonProperty("guild_member")]
    public GuildMemberJson? Member { get; set; }

    [JsonProperty("guild_member_profile")]
    public UserProfileJson? MemberProfile { get; set; }

    [JsonProperty("premium_type")]
    public int PremiumType { get; set; }

    [JsonProperty("premium_since")]
    public DateTimeOffset? PremiumSince { get; set; }

    [JsonProperty("mutual_friends")]
    public UserJson[]? MutualFriends { get; set; }

    [JsonProperty("mutual_guilds")]
    public GuildJson[]? MutualGuilds { get; set; }

    [JsonProperty("connected_accounts")]
    public UserConnectionJson[] Connections { get; set; }
}
