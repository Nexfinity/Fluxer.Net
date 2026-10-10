using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Currently only used when updating role positions in <see cref="FluxerApiClient.UpdateRolePositionsAsync(ulong, IEnumerable{Fluxer.Net.RolePositionItemJson})"/>
/// </summary>
public class RolePositionItemJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("position")]
    public int? Position { get; set; }
}
