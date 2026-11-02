using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class PartialInviteJson : IPartialInvite
{
    /// <inheritdoc />
    [JsonProperty("code")]
    public string Code { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public InviteType Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild")]
    public PartialGuildJson? Guild { get; set; }

    /// <inheritdoc />
    [JsonProperty("channel")]
    public PartialChannelJson? Channel { get; set; }

    /// <inheritdoc />
    [JsonProperty("inviter")]
    public UserJson? Inviter { get; set; }

    /// <inheritdoc />
    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    /// <inheritdoc />
    [JsonProperty("presence_count")]
    public int PresenceCount { get; set; }

    /// <inheritdoc />
    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("temporary")]
    public bool IsTemporary { get; set; }

    IPartialGuild? IPartialInvite.Guild => Guild;

    IPartialChannel? IPartialInvite.Channel => Channel;

    IUser IPartialInvite.Inviter => Inviter;
}