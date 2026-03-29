using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildTransferOwnershipRequest
{
    [JsonProperty("new_owner_id")]
    public ulong NewOwnerId { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }
}
