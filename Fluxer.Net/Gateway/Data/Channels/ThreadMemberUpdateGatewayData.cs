using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ThreadMemberUpdateGatewayData : ThreadMemberJson
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }
}
