using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A page of audit log entries with the users and webhooks they reference.
/// </summary>
public class AuditLogListJson
{
    /// <summary>
    /// The audit log entries.
    /// </summary>
    [JsonProperty("audit_log_entries")]
    public AuditLogJson[] Entries { get; set; } = Array.Empty<AuditLogJson>();

    /// <summary>
    /// The users referenced by the entries.
    /// </summary>
    [JsonProperty("users")]
    public UserJson[] Users { get; set; } = Array.Empty<UserJson>();

    /// <summary>
    /// The webhooks referenced by the entries.
    /// </summary>
    [JsonProperty("webhooks")]
    public AuditLogWebhookJson[] Webhooks { get; set; } = Array.Empty<AuditLogWebhookJson>();
}
