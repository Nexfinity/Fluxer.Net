using Newtonsoft.Json;
using Fluxer.Net.Objects.Data;

namespace Fluxer.Net.Objects;

public class Profile
{
    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("user_profile")]
    public UserProfile UserProfile { get; set; }

    [JsonProperty("premium_type")]
    public PremiumType PremiumType { get; set; }

    [JsonProperty("premium_since")]
    public DateTime PremiumSince { get; set; }

    [JsonProperty("timezone_offset")]
    public int TimezoneOffset { get; set; }

    [JsonProperty("birthday")]
    public DateOnly Birthday { get; set; }

    [JsonProperty("guild_member")]
    public GuildMember GuildMember { get; set; }

    [JsonProperty("guild_member_profile")]
    public GuildMemberProfile GuildMemberProfile { get; set; }


}
