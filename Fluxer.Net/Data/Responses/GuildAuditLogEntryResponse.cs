using Fluxer.Net.Data.Enums;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L71"/>
/// </remarks>
public class GuildAuditLogEntryResponse
{
    [JsonRequired]
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonPropertyName("action_type")]
    public AuditLogActionType ActionType { get; set; }

    [JsonPropertyName("user_id")]
    public ulong? UserId { get; set; }

    [JsonPropertyName("target_id")]
    public ulong? TargetId { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("options")]
    public AuditLogResponseItemOptions? Options { get; set; }

    [JsonPropertyName("changes")]
    public AuditLogResponseItemChangeBase[]? Changes { get; set; }
}

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L54C7-L54C28"/>
/// </remarks>
public class AuditLogResponseItemOptions
{
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("delete_member_days")]
    public int? DeleteMemberDays { get; set; }

    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("integration_type")]
    public int? IntegrationType { get; set; }

    [JsonPropertyName("message_id")]
    public ulong? MessageId { get; set; }

    [JsonPropertyName("members_removed")]
    public int? MembersRemoved { get; set; }

    [JsonPropertyName("role_name")]
    public string? RoleName { get; set; }

    [JsonPropertyName("type")]
    public int? Type { get; set; }

    [JsonPropertyName("inviter_id")]
    public ulong? InviterId { get; set; }

    [JsonPropertyName("max_age")]
    public int? MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public int? MaxUses { get; set; }

    [JsonPropertyName("temporary")]
    public bool? Temporary { get; set; }

    [JsonPropertyName("uses")]
    public int? Uses { get; set; }
}
