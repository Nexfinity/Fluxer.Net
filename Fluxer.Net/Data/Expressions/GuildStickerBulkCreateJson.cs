using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The result of creating several guild stickers at once.
/// </summary>
public class GuildStickerBulkCreateJson
{
    /// <summary>
    /// The stickers that were created.
    /// </summary>
    [JsonProperty("success")]
    public GuildStickerJson[] Success { get; set; }

    /// <summary>
    /// The stickers that could not be created.
    /// </summary>
    [JsonProperty("failed")]
    public GuildStickerBulkCreateFailureItemJson[] Failed { get; set; } = Array.Empty<GuildStickerBulkCreateFailureItemJson>();
}

/// <summary>
/// A sticker that could not be created in a bulk create request.
/// </summary>
public class GuildStickerBulkCreateFailureItemJson
{
    /// <summary>
    /// The name of the sticker.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The error reported for the sticker.
    /// </summary>
    [JsonProperty("error")]
    public string Error { get; set; }
}