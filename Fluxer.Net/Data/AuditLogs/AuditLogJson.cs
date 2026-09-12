using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildAuditLogJson : AuditLogJson
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }
}
public class AuditLogJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("action_type")]
    public ActionType Action { get; set; }

    [JsonProperty("user_id")]
    public ulong? UserId { get; set; }

    [JsonProperty("target_id")]
    public ulong? TargetId { get; set; }

    [JsonProperty("reason")]
    public string? Reason { get; set; }

    [JsonProperty("options")]
    public AuditLogOptionsJson? Options { get; set; }

    [JsonProperty("changes")]
    public AuditLogChangeJson[]? Changes { get; set; }
}