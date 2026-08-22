using Newtonsoft.Json;

namespace Fluxer.Net;

public class ReadStateJson
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong? LastMessageId { get; set; }

    [JsonProperty("mention_count")]
    public int MentionCount { get; set; }

    [JsonProperty("last_pin_timestamp")]
    public DateTimeOffset? LastPinAt { get; set; }
}
