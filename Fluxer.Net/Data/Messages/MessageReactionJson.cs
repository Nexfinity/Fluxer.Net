using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageReactionJson
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("bucket")]
    public int Bucket { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("emoji_id")]
    public ulong EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public string EmojiName { get; set; }

    [JsonProperty("emoji_animated")]
    public bool IsEmojiAnimated { get; set; }
}
