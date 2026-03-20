using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildVanityUrlJson : IGuildVanityUrl
{

    /// <inheritdoc />
    [JsonProperty("code")]
    public string? Code { get; set; }


    /// <inheritdoc />
    [JsonProperty("uses")]
    public int Uses { get; set; }
}
