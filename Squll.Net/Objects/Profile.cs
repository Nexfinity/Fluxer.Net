using Newtonsoft.Json;
using Squll.Net.Objects.Data;

namespace Squll.Net.Objects;

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

    [JsonProperty("squad_member")]
    public SquadMember SquadMember { get; set; }

    [JsonProperty("squad_member_profile")]
    public SquadMemberProfile SquadMemberProfile { get; set; }


}
