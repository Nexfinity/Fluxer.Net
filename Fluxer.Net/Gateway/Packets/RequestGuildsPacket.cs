using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class RequestGuildsPacket
{
    [JsonProperty("guild_ids")]
    public string[] GuildIds { get; set; }
}
