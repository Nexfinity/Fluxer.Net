using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class PresenceUpdatedBulkGatewayData
{
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("presences")]
    public PresenceGatewayData[] Presences { get; set; }
}
