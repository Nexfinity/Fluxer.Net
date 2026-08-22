using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class InviteJson : PartialInviteJson, IInvite
{
    /// <inheritdoc />
    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("uses")]
    public int Uses { get; set; }

    /// <inheritdoc />
    [JsonProperty("max_uses")]
    public int MaxUses { get; set; }

    /// <inheritdoc />
    [JsonProperty("max_age")]
    public int MaxAge { get; set; }
}
