using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Changes who can see a connection or where it is listed.
/// </summary>
public class UpdateConnectionRequest
{
    /// <summary>
    /// Bitfield controlling who can see the connection.
    /// </summary>
    [JsonProperty("visibility_flags")]
    public int? VisibilityFlags { get; set; }

    /// <summary>
    /// The display order of the connection.
    /// </summary>
    [JsonProperty("sort_order")]
    public int? SortOrder { get; set; }
}
