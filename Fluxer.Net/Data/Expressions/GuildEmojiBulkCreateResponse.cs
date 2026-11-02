using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The result of creating several guild emojis at once.
/// </summary>
public class GuildEmojiBulkCreateJson
{
    /// <summary>
    /// The emojis that were created.
    /// </summary>
    [JsonProperty("success")]
    public GuildEmojiJson[] Success { get; set; } = Array.Empty<GuildEmojiJson>();

    /// <summary>
    /// The emojis that could not be created.
    /// </summary>
    [JsonProperty("failed")]
    public GuildEmojiBulkCreateFailureItemJson[] Failed { get; set; } = Array.Empty<GuildEmojiBulkCreateFailureItemJson>();
}

/// <summary>
/// An emoji that could not be created in a bulk create request.
/// </summary>
public class GuildEmojiBulkCreateFailureItemJson
{
    /// <summary>
    /// The name of the emoji.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The error reported for the emoji.
    /// </summary>
    [JsonProperty("error")]
    public string Error { get; set; }
}
