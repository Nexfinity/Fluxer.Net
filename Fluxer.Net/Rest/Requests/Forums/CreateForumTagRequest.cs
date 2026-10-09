using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreateForumTagRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("moderated")]
    public bool? IsModerated { get; set; }

    [JsonProperty("emoji_name")]
    public string? EmojiName { get; set; }

    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }
}
