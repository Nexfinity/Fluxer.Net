using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class RtcRegionJson : IRtcRegion
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("emoji")]
    public string Emoji { get; set; }
}
