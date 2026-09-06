using Newtonsoft.Json;

namespace Fluxer.Net;

public class PartialInviteJson : IPartialInvite
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("type")]
    public InviteType Type { get; set; }

    [JsonProperty("guild")]
    public PartialGuildJson? Guild { get; set; }

    [JsonProperty("channel")]
    public PartialChannelJson? Channel { get; set; }

    [JsonProperty("inviter")]
    public UserJson Inviter { get; set; }

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("presence_count")]
    public int PresenceCount { get; set; }

    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonProperty("temporary")]
    public bool IsTemporary { get; set; }

    IPartialGuild? IPartialInvite.Guild => Guild;

    IPartialChannel? IPartialInvite.Channel => Channel;

    IUser IPartialInvite.Inviter => Inviter;
}