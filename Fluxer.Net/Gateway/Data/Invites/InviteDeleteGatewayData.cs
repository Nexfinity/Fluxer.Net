using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class InviteDeleteGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }
}