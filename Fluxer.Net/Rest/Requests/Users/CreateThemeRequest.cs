using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Stores a theme so it can be shared.
/// </summary>
public class CreateThemeRequest
{
    /// <summary>
    /// The CSS text of the theme.
    /// </summary>
    [JsonProperty("css")]
    public string Css { get; set; }
}
