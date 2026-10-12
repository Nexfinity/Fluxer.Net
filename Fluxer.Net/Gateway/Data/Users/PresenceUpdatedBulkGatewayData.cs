using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class PresenceUpdatedBulkGatewayData
{
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("presences")]
    public PresenceGatewayData[] Presences { get; set; }
}