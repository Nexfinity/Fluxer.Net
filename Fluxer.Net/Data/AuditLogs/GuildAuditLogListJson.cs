using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L94"/>
/// </remarks>
public class GuildAuditLogListJson
{
    [JsonProperty("audit_log_entries")]
    public GuildAuditLogEntryJson[] Entries { get; set; } = Array.Empty<GuildAuditLogEntryJson>();

    [JsonProperty("users")]
    public UserPartialResponse[] Users { get; set; } = Array.Empty<UserPartialResponse>();

    [JsonProperty("webhooks")]
    public AuditLogWebhookJson[] Webhooks { get; set; } = Array.Empty<AuditLogWebhookJson>();
}
