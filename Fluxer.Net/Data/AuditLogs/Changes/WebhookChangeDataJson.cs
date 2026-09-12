using Newtonsoft.Json;

namespace Fluxer.Net;

public class WebhookChangeDataJson : IAuditLogData
{
    [JsonProperty("id")]
    public ulong WebhookId { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("creator_id")]
    public ulong? CreatorId { get; set; }

    [JsonProperty("avatar_hash")]
    public string? AvatarHash { get; set; }
}
