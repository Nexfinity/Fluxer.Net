using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CloneGuildExpressionRequest
{
    [JsonProperty("source_emoji_id")]
    public ulong? SourceEmojiId { get; set; }

    [JsonProperty("source_sticker_id")]
    public ulong? SourceStickerId { get; set; }
}
