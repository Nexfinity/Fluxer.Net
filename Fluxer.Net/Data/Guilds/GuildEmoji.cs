using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildEmoji : Entity
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("creator_id")]
    public ulong CreatorId { get; set; }

    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }
}
