using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class GuildBanGatewayData : IGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("user")]
    public UserJson User { get; set; }
}
