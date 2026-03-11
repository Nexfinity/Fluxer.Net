using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildAuditLog : Entity
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("log_id")]
    public ulong LogId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("target_type")]
    public string TargetType { get; set; }

    [JsonProperty("target_id")]
    public string? TargetId { get; set; }

    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("audit_log_reason")]
    public string? AuditLogReason { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }

    [JsonProperty("changes")]
    public object? Changes { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
}
