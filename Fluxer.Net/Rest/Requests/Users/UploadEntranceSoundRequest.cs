using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Adds a short audio clip to the entrance sound library.
/// </summary>
public class UploadEntranceSoundRequest
{
    /// <summary>
    /// The display label.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The base64-encoded audio bytes.
    /// </summary>
    [JsonProperty("audio")]
    public string Audio { get; set; }
}
