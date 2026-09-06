using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class MessageDeleteGatewayData
{
    [JsonProperty("id")]
    public ulong MessageId { get; set; }

    [JsonProperty("author_id")]
    public ulong? AuthorId { get; set; }

    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}
