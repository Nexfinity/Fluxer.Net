using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a private thread.
/// </summary>
public class CreatePrivateThreadRequest : CreateThreadRequest
{
    /// <summary>
    /// The channel type, always a private thread.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.PrivateThread;
    /// <summary>
    /// Whether members who are not thread moderators can add members, on a private thread (null = true)
    /// </summary>
    [JsonProperty("invitable")]
    public bool? IsInvitable { get; set; }
}
