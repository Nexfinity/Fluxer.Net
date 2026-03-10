using Newtonsoft.Json;

namespace Fluxer.Net.Data.Messages;

public class SavedMessage
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("saved_at")]
    public DateTime SavedAt { get; set; }
}
