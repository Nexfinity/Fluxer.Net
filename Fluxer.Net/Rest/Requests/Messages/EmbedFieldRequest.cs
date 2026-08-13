using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class EmbedFieldRequest
{
    /// <summary>
    /// Name of the field.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Value of the field (1-1024 characters)
    /// </summary>
    [JsonProperty("value")]
    public string Value { get; set; }

    /// <summary>
    /// Whether the field should display inline.
    /// </summary>
    [JsonProperty("inline")]
    public bool IsInline { get; set; }
}
