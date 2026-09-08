using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class MessageReactionsRemoveGatewayData
{
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}