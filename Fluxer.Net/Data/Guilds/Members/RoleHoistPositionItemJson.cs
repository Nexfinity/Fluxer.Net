using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A role and the hoist position to set for it, used when updating hoist positions.
/// </summary>
public class RoleHoistPositionItemJson
{
    /// <summary>
    /// The ID of the role.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The hoist position to set for the role.
    /// </summary>
    [JsonProperty("hoist_position")]
    public int HoistPosition { get; set; }
}
