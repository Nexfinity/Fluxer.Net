using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Currently only used when updating role positions in <see cref="ApiClient.UpdateRolePositions"/>
/// </summary>
public class GuildRolePositionItem
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("position")]
    public int? Position { get; set; }
}
