using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Tells the gif provider that a gif was shared.
/// </summary>
public class RegisterGifShareRequest
{
    /// <summary>
    /// The provider-issued share identifier, the slug of the gif.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// The search query that produced the gif.
    /// </summary>
    [JsonProperty("q")]
    public string? Query { get; set; }

    /// <summary>
    /// The locale of the client, such as en-US.
    /// </summary>
    [JsonProperty("locale")]
    public string? Locale { get; set; }
}
