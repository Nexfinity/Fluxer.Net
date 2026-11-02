using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a guild.
/// </summary>
public class CreateGuildRequest
{
    /// <summary>
    /// Whether the guild is created with no features enabled.
    /// </summary>
    [JsonProperty("empty_features")]
    public bool? EmptyFeatures { get; set; }

    /// <summary>
    /// Base64 image data for the guild icon, or null for no icon.
    /// </summary>
    [JsonProperty("icon")]
    public string? IconBase64 { get; set; }

    /// <summary>
    /// The name of the guild.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Sets <see cref="IconBase64"/> from an image stream.
    /// </summary>
    public void IconFromStream(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        IconBase64 = Convert.ToBase64String(ms.ToArray());
    }
}
