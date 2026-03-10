using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.AuditLogs;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L94"/>
/// </remarks>
public class GuildAuditLogListResponse : Entity
{
    [JsonProperty("audit_log_entries")]
    public GuildAuditLogEntryResponse[] Entries { get; set; } = Array.Empty<GuildAuditLogEntryResponse>();

    [JsonProperty("users")]
    public UserPartialResponse[] Users { get; set; } = Array.Empty<UserPartialResponse>();

    [JsonProperty("webhooks")]
    public AuditLogWebhookResponse[] Webhooks { get; set; } = Array.Empty<AuditLogWebhookResponse>();
}
