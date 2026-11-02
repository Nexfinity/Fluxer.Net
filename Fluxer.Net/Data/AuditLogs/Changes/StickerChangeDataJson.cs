using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class StickerChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the sticker.
    /// </summary>
    [JsonProperty("sticker_id")]
    public ulong StickerId { get; set; }

    /// <summary>
    /// The name of the sticker.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The description of the sticker.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether the sticker is animated.
    /// </summary>
    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }

    /// <summary>
    /// The id of the user that uploaded the sticker.
    /// </summary>
    [JsonProperty("creator_id")]
    public ulong CreatorId { get; set; }
}
