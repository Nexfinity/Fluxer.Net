using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating an emoji in a guild.
/// </summary>
public class UpdateGuildEmojiRequest
{
    /// <summary>
    /// The new name of the emoji.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }
}
