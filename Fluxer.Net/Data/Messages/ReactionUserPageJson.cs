using Newtonsoft.Json;

namespace Fluxer.Net;

public class ReactionUserPageJson
{
    [JsonProperty("items")]
    public UserJson[] Users { get; set; } = Array.Empty<UserJson>();

    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("next_after")]
    public ulong? NextAfterId { get; set; }
}
