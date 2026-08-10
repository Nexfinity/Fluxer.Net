using Newtonsoft.Json;

namespace Fluxer.Net;

public class ChannelPositionUpdateRequestItem
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("lock_permissions")]
    public bool? LockPermissions { get; set; }

    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    [JsonProperty("position")]
    public int? Position { get; set; }

    [JsonProperty("preceding_sibling_id")]
    public ulong? PrecedingSiblingId { get; set; }
}
