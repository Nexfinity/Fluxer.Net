using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildAuditLogListRequest
{
    /// <summary>
    /// Maximum number of audit log entries to return (1-100)
    /// </summary>
    [JsonProperty("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Get entries before this audit log entry ID
    /// </summary>
    [JsonProperty("before")]
    public ulong? Before { get; set; }

    /// <summary>
    /// Get entries after this audit log entry ID
    /// </summary>
    [JsonProperty("after")]
    public ulong? After { get; set; }

    /// <summary>
    /// Filter entries by the user who performed the action
    /// </summary>
    [JsonProperty("user")]
    public ulong? UserId { get; set; }

    /// <summary>
    /// Filter entries by the type of action
    /// </summary>
    [JsonProperty("action_type")]
    public AuditLogActionType? ActionType { get; set; }
}
