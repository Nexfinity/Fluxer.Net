using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A role and the position to set for it, used when updating role positions with <see cref="FluxerApiClient.UpdateRolePositionsAsync"/>.
/// </summary>
public class RolePositionItemJson
{
    /// <summary>
    /// The ID of the role.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The position to set for the role.
    /// </summary>
    [JsonProperty("position")]
    public int? Position { get; set; }
}
