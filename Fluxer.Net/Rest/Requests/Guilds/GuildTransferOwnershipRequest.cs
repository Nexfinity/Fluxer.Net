using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for transferring ownership of a guild.
/// </summary>
public class GuildTransferOwnershipRequest
{
    /// <summary>
    /// The ID of the member who becomes the owner.
    /// </summary>
    [JsonProperty("new_owner_id")]
    public ulong NewOwnerId { get; set; }

    /// <summary>
    /// Account password for sudo verification.
    /// </summary>
    [JsonProperty("password")]
    public string? Password { get; set; }
}
