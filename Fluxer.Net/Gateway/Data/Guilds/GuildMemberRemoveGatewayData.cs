using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class GuildMemberRemoveGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("user")]
    public GuildMemberUserRemoveGatewayData User { get; set; }
}
public class GuildMemberUserRemoveGatewayData
{
    [JsonProperty("id")]
    public ulong Id { get; set; }
}