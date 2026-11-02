using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// An audit log entry of a guild, as the API sends it.
/// </summary>
public class GuildAuditLogJson : AuditLogJson
{
    /// <summary>
    /// The id of the guild the entry belongs to.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }
}
/// <summary>
/// An audit log entry, as the API sends it.
/// </summary>
public class AuditLogJson
{
    /// <summary>
    /// The id of the entry.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The type of action the entry records.
    /// </summary>
    [JsonProperty("action_type")]
    public ActionType Action { get; set; }

    /// <summary>
    /// The id of the user that made the change.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong? UserId { get; set; }

    /// <summary>
    /// The id of the entity that was changed.
    /// </summary>
    [JsonProperty("target_id")]
    public ulong? TargetId { get; set; }

    /// <summary>
    /// The reason given for the change.
    /// </summary>
    [JsonProperty("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Extra details about the action.
    /// </summary>
    [JsonProperty("options")]
    public AuditLogOptionsJson? Options { get; set; }

    /// <summary>
    /// The fields that changed, with their values before and after the change.
    /// </summary>
    [JsonProperty("changes")]
    public AuditLogChangeJson[]? Changes { get; set; }
}