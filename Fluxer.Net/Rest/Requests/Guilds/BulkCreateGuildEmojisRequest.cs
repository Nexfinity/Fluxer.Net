using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class BulkCreateGuildEmojisRequest
{
    [JsonProperty("emojis")]
    public CreateGuildEmojiRequest[] Emojis { get; set; } = Array.Empty<CreateGuildEmojiRequest>();
}
