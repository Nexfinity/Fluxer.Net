using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class ChannelPositionUpdateRequestItem
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }
    
    [JsonPropertyName("lock_permissions")]
    public bool? LockPermissions { get; set; }
    
    [JsonPropertyName("parent_id")]
    public ulong? ParentId { get; set; }
    
    [JsonPropertyName("position")]
    public int? Position { get; set; }
    
    [JsonPropertyName("preceding_sibling_id")]
    public ulong? PrecedingSiblingId { get; set; }
}
