using Newtonsoft.Json;

namespace Fluxer.Net;

public class PartialInvite : Entity
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("guild")]
    public PartialGuild? Guild { get; set; }

    [JsonProperty("channel")]
    public InviteChannel? Channel { get; set; }

    [JsonProperty("inviter")]
    public InviteUser Inviter { get; set; }

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("presence_count")]
    public int PresenceCount { get; set; }

    [JsonProperty("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [JsonProperty("temporary")]
    public bool Temporary { get; set; }
}
