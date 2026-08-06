using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildAuditLogListJson
{
    [JsonProperty("audit_log_entries")]
    public GuildAuditLogEntryJson[] Entries { get; set; } = Array.Empty<GuildAuditLogEntryJson>();

    [JsonProperty("users")]
    public UserJson[] Users { get; set; } = Array.Empty<UserJson>();

    [JsonProperty("webhooks")]
    public AuditLogWebhookJson[] Webhooks { get; set; } = Array.Empty<AuditLogWebhookJson>();
}
