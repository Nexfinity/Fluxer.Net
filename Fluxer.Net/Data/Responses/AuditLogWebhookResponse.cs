using Fluxer.Net.Data.Enums;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L83"/>
/// </remarks>
public class AuditLogWebhookResponse
{
    [JsonRequired]
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonPropertyName("type")]
    public WebhookType Type { get; set; }
    
    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("avatar_hash")]
    public string? AvatarHash { get; set; }
}
