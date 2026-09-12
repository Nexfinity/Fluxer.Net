using Newtonsoft.Json;

namespace Fluxer.Net;

public class InviteChangeDataJson : IAuditLogData
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }

    [JsonProperty("uses")]
    public int? Uses { get; set; }

    [JsonProperty("max_uses")]
    public int? MaxUses { get; set; }

    [JsonProperty("max_age")]
    public int? MaxAge { get; set; }

    [JsonProperty("temporary")]
    public bool? IsTemporary { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
