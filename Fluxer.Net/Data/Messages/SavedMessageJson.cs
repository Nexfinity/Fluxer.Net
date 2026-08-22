using Newtonsoft.Json;

namespace Fluxer.Net;

public class SavedMessageJson
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("saved_at")]
    public DateTimeOffset SavedAt { get; set; }
}
