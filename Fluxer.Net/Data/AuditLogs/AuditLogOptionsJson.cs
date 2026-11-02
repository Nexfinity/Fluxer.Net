using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Extra details of an audit log entry, such as the channel or the number of items it affected.
/// </summary>
public class AuditLogOptionsJson
{
    /// <summary>
    /// The channel the action applied to.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// The number of items the action affected.
    /// </summary>
    [JsonProperty("count")]
    public int? Count { get; set; }

    /// <summary>
    /// The number of days of the member's messages to delete.
    /// </summary>
    [JsonProperty("delete_member_days")]
    public int? DeleteMemberDays { get; set; }

    /// <summary>
    /// The id of the entity the action applied to.
    /// </summary>
    [JsonProperty("id")]
    public ulong? Id { get; set; }

    /// <summary>
    /// The type of the integration the action applied to, as a number.
    /// </summary>
    [JsonProperty("integration_type")]
    public int? IntegrationType { get; set; }

    /// <summary>
    /// The id of the message the action applied to.
    /// </summary>
    [JsonProperty("message_id")]
    public ulong? MessageId { get; set; }

    /// <summary>
    /// The number of members removed.
    /// </summary>
    [JsonProperty("members_removed")]
    public int? MembersRemoved { get; set; }

    /// <summary>
    /// The name of the role the action applied to.
    /// </summary>
    [JsonProperty("role_name")]
    public string? RoleName { get; set; }

    /// <summary>
    /// The type of the entity the action applied to, as a number.
    /// </summary>
    [JsonProperty("type")]
    public int? Type { get; set; }

    /// <summary>
    /// The id of the user that created the invite.
    /// </summary>
    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }

    /// <summary>
    /// The duration in seconds before the invite expires.
    /// </summary>
    [JsonProperty("max_age")]
    public int? MaxAge { get; set; }

    /// <summary>
    /// The maximum number of uses of the invite.
    /// </summary>
    [JsonProperty("max_uses")]
    public int? MaxUses { get; set; }

    /// <summary>
    /// Whether the invite grants temporary membership.
    /// </summary>
    [JsonProperty("temporary")]
    public bool? Temporary { get; set; }

    /// <summary>
    /// The number of times the invite was used.
    /// </summary>
    [JsonProperty("uses")]
    public int? Uses { get; set; }
}