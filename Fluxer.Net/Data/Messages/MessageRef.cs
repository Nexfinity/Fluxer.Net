using Newtonsoft.Json;

namespace Fluxer.Net.Data.Messages;

// TODO MessageReferenceResponse should be used instead, and a seperate gateway message/model if it's different in any way
public class MessageRef
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }
}
