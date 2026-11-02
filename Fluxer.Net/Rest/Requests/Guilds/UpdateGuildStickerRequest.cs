using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating a sticker in a guild.
/// </summary>
public class UpdateGuildStickerRequest
{
    /// <summary>
    /// The name of the sticker.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The description of the sticker.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The tags that describe the sticker.
    /// </summary>
    [JsonProperty("tags")]
    public string[]? Tags { get; set; }
}
