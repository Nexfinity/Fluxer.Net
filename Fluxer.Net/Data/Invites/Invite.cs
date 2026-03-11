using Newtonsoft.Json;

namespace Fluxer.Net;

public class Invite : Entity
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("uses")]
    public int Uses { get; set; }

    [JsonProperty("max_uses")]
    public int MaxUses { get; set; }

    [JsonProperty("max_age")]
    public int MaxAge { get; set; }

    [JsonProperty("temporary")]
    public bool Temporary { get; set; }
}
