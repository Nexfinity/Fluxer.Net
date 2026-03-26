using Newtonsoft.Json;

namespace Fluxer.Net;

public class EmojiJson : IEmoji
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }
}
