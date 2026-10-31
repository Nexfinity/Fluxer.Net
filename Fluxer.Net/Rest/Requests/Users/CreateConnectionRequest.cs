using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Starts adding an external connection to your profile.
/// </summary>
public class CreateConnectionRequest
{
    /// <summary>
    /// "bluesky" or "domain".
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// The handle or domain to connect.
    /// </summary>
    [JsonProperty("identifier")]
    public string Identifier { get; set; }

    /// <summary>
    /// Bitfield controlling who can see the connection.
    /// </summary>
    [JsonProperty("visibility_flags")]
    public int? VisibilityFlags { get; set; }
}
