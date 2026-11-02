using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class InviteChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The invite code.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; }

    /// <summary>
    /// The id of the channel the invite is for.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// The id of the guild the invite is for.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The id of the user that created the invite.
    /// </summary>
    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }

    /// <summary>
    /// The number of times the invite was used.
    /// </summary>
    [JsonProperty("uses")]
    public int? Uses { get; set; }

    /// <summary>
    /// The maximum number of uses of the invite.
    /// </summary>
    [JsonProperty("max_uses")]
    public int? MaxUses { get; set; }

    /// <summary>
    /// The duration in seconds before the invite expires.
    /// </summary>
    [JsonProperty("max_age")]
    public int? MaxAge { get; set; }

    /// <summary>
    /// Whether the invite grants temporary membership.
    /// </summary>
    [JsonProperty("temporary")]
    public bool? IsTemporary { get; set; }

    /// <summary>
    /// When the invite was created.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
