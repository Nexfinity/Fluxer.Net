using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for copying an emoji or a sticker into a guild.
/// </summary>
public class CloneGuildExpressionRequest
{
    /// <summary>
    /// The ID of the emoji to copy.
    /// </summary>
    [JsonProperty("source_emoji_id")]
    public ulong? SourceEmojiId { get; set; }

    /// <summary>
    /// The ID of the sticker to copy.
    /// </summary>
    [JsonProperty("source_sticker_id")]
    public ulong? SourceStickerId { get; set; }
}
