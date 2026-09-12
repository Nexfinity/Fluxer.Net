using Newtonsoft.Json;

namespace Fluxer.Net;

public class AuditLogListJson
{
    [JsonProperty("audit_log_entries")]
    public AuditLogJson[] Entries { get; set; } = Array.Empty<AuditLogJson>();

    [JsonProperty("users")]
    public UserJson[] Users { get; set; } = Array.Empty<UserJson>();

    [JsonProperty("webhooks")]
    public AuditLogWebhookJson[] Webhooks { get; set; } = Array.Empty<AuditLogWebhookJson>();
}
