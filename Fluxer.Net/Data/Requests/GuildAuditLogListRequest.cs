using Fluxer.Net.Data.Enums;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class GuildAuditLogListRequest
{
    /// <summary>
    /// Maximum number of audit log entries to return (1-100)
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Get entries before this audit log entry ID
    /// </summary>
    [JsonPropertyName("before")]
    public ulong? Before { get; set; }

    /// <summary>
    /// Get entries after this audit log entry ID
    /// </summary>
    [JsonPropertyName("after")]
    public ulong? After { get; set; }

    /// <summary>
    /// Filter entries by the user who performed the action
    /// </summary>
    [JsonPropertyName("user")]
    public ulong? UserId { get; set; }

    /// <summary>
    /// Filter entries by the type of action
    /// </summary>
    [JsonPropertyName("action_type")]
    public AuditLogActionType? ActionType { get; set; }
}
