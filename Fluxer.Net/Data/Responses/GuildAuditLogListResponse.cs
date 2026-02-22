using Fluxer.Net.Gateway.Data;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L94"/>
/// </remarks>
public class GuildAuditLogListResponse
{
    [JsonPropertyName("audit_log_entries")]
    public GuildAuditLogEntryResponse[] Entries { get; set; } = Array.Empty<GuildAuditLogEntryResponse>();

    [JsonPropertyName("users")]
    public UserPartialResponse[] Users { get; set; } = Array.Empty<UserPartialResponse>();

    [JsonPropertyName("webhooks")]    
    public AuditLogWebhookResponse[] Webhooks { get; set; } = Array.Empty<AuditLogWebhookResponse>();
}
