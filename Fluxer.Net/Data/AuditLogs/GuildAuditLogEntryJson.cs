using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L71"/>
/// </remarks>
public class GuildAuditLogEntryJson
{
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonProperty("action_type")]
    public AuditLogActionType ActionType { get; set; }

    [JsonProperty("user_id")]
    public ulong? UserId { get; set; }

    [JsonProperty("target_id")]
    public ulong? TargetId { get; set; }

    [JsonProperty("reason")]
    public string? Reason { get; set; }

    [JsonProperty("options")]
    public AuditLogResponseItemOptions? Options { get; set; }

    [JsonProperty("changes")]
    public AuditLogResponseItemChangeJson[]? Changes { get; set; }
}

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L54C7-L54C28"/>
/// </remarks>
public class AuditLogResponseItemOptions
{
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonProperty("count")]
    public int? Count { get; set; }

    [JsonProperty("delete_member_days")]
    public int? DeleteMemberDays { get; set; }

    [JsonProperty("id")]
    public ulong? Id { get; set; }

    [JsonProperty("integration_type")]
    public int? IntegrationType { get; set; }

    [JsonProperty("message_id")]
    public ulong? MessageId { get; set; }

    [JsonProperty("members_removed")]
    public int? MembersRemoved { get; set; }

    [JsonProperty("role_name")]
    public string? RoleName { get; set; }

    [JsonProperty("type")]
    public int? Type { get; set; }

    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }

    [JsonProperty("max_age")]
    public int? MaxAge { get; set; }

    [JsonProperty("max_uses")]
    public int? MaxUses { get; set; }

    [JsonProperty("temporary")]
    public bool? Temporary { get; set; }

    [JsonProperty("uses")]
    public int? Uses { get; set; }
}
