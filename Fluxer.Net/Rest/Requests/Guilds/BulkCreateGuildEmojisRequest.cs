using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating several emojis in a guild.
/// </summary>
public class BulkCreateGuildEmojisRequest
{
    /// <summary>
    /// The emojis to create in the guild.
    /// </summary>
    [JsonProperty("emojis")]
    public CreateGuildEmojiRequest[] Emojis { get; set; } = Array.Empty<CreateGuildEmojiRequest>();
}
