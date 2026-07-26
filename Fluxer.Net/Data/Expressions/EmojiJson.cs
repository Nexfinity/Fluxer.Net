using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
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

    /// <inheritdoc />
    [JsonProperty("allow_cloning")]
    public bool AllowCloning { get; set; }
}
