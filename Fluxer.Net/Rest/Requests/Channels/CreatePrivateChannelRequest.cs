using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for opening a direct or group message channel.
/// </summary>
public class CreatePrivateChannelRequest
{
    /// <summary>
    /// The ID of the user to open a direct message channel with.
    /// </summary>
    [JsonProperty("recipient_id")]
    public ulong? RecipientId { get; set; }

    /// <summary>
    /// The IDs of the users to open a group message channel with.
    /// </summary>
    [JsonProperty("recipients")]
    public HashSet<ulong>? Recipients { get; set; }
}
