using Newtonsoft.Json;

namespace Fluxer.Net;

public class AuditLogWebhookJson
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
