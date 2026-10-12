using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildMemberRemoveGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("user")]
    public GuildMemberUserRemoveGatewayData User { get; set; }
}

internal class GuildMemberUserRemoveGatewayData
{
    [JsonProperty("id")]
    public ulong Id { get; set; }
}