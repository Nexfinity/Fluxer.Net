using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class GuildBanGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("user")]
    public UserJson User { get; set; }
}
