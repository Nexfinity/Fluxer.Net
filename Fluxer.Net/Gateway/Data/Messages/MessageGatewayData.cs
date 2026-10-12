using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class MessageGatewayData : MessageJson
{
    [JsonProperty("channel_type")]
    public ChannelType ChannelType { get; set; }

    [JsonProperty("member")]
    public GuildMemberJson? Member { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}