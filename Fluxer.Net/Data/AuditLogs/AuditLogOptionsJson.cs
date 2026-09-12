using Newtonsoft.Json;

namespace Fluxer.Net;

public class AuditLogOptionsJson
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