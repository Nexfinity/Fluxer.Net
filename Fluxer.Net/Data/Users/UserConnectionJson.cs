using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class UserConnectionJson : IUserConnection
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("verified")]
    public bool IsVerified { get; set; }

    /// <inheritdoc />
    [JsonProperty("visibility_flags")]
    public ulong VisibilityFlags { get; set; }

    /// <inheritdoc />
    [JsonProperty("sort_order")]
    public int SortOrder { get; set; }
}
