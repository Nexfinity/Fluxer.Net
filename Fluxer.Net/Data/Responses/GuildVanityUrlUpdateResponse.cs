using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

public class GuildVanityUrlUpdateResponse
{
    /// <summary>
    /// The new vanity Url code
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("code")]
    public string Code { get; set; }
}
