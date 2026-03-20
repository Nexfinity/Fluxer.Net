using Newtonsoft.Json;

namespace Fluxer.Net;

public class PartialInviteJson : IPartialInvite
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("guild")]
    public PartialGuildJson? Guild { get; set; }

    [JsonProperty("channel")]
    public InviteChannelJson? Channel { get; set; }

    [JsonProperty("inviter")]
    public InviteUserJson Inviter { get; set; }

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("presence_count")]
    public int PresenceCount { get; set; }

    [JsonProperty("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [JsonProperty("temporary")]
    public bool Temporary { get; set; }
}
