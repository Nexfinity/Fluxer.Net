using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Models;

/// <summary>
/// Currently only used when updating role positions in <see cref="ApiClient.UpdateRolePositions"/>
/// </summary>
public class GuildRolePositionItem
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }
}
