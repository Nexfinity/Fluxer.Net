using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating several stickers in a guild.
/// </summary>
public class BulkCreateGuildStickersRequest
{
    /// <summary>
    /// The stickers to create in the guild.
    /// </summary>
    [JsonProperty("stickers")]
    public CreateGuildStickerRequest[] Stickers { get; set; } = Array.Empty<CreateGuildStickerRequest>();
}
