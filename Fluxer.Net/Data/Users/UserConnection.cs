using Newtonsoft.Json;

namespace Fluxer.Net;

public class UserConnection : Entity
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("verified")]
    public bool IsVerified { get; set; }

    [JsonProperty("visibility_flags")]
    public ulong VisibilityFlags { get; set; }

    [JsonProperty("sort_order")]
    public int SortOrder { get; set; }
}
