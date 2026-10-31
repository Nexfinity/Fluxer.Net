using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Changes the display label of an entrance sound.
/// </summary>
public class RenameEntranceSoundRequest
{
    /// <summary>
    /// The new display label.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }
}
