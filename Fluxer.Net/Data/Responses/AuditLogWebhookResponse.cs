using Fluxer.Net.Data.Enums;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L83"/>
/// </remarks>
public class AuditLogWebhookResponse
{
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonProperty("type")]
    public WebhookType Type { get; set; }
    
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("avatar_hash")]
    public string? AvatarHash { get; set; }
}
