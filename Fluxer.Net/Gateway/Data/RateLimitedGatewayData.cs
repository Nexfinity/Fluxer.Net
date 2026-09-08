using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class RateLimitedGatewayData
{
    [JsonProperty("opcode")]
    public int Opcode { get; set; }

    [JsonProperty("retry_after")]
    public int RetryAfter { get; set; }

    [JsonProperty("meta")]
    public RateLimitedMetaGatewayData Meta { get; set; }
}
public class RateLimitedMetaGatewayData
{
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}
