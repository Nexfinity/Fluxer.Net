using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildHealthUpdateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("degraded")]
    public bool IsDegraded { get; set; }
}
