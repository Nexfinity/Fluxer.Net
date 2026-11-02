using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The result of a username availability check.
/// </summary>
public class UsernameAvailableJson
{
    /// <summary>
    /// Whether the username is taken.
    /// </summary>
    [JsonProperty("taken")]
    public bool Taken { get; set; }
}
