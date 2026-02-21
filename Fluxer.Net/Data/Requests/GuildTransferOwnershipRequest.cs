using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class GuildTransferOwnershipRequest
{
    [JsonPropertyName("new_owner_id")]
    public ulong NewOwnerId { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
