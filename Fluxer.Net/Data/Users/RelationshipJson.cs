using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class RelationshipJson : IRelationship
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong UserId { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public RelationshipType Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("nickname")]
    public string? Nickname { get; set; }

    /// <inheritdoc />
    [JsonProperty("since")]
    public DateTimeOffset? SinceAt { get; set; }
}
