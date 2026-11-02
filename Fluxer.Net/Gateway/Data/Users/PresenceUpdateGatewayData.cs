using Newtonsoft.Json;
using StatusEnum = Fluxer.Net.Status;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The presence the session sends when it identifies or changes its presence.
/// </summary>
public class PresenceUpdateGatewayData
{
    /// <summary>
    /// The status name: online, idle, dnd or invisible. Null when no status is set.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Whether the session is on a mobile client.
    /// </summary>
    [JsonProperty("mobile")]
    public bool? IsMobile { get; set; }

    /// <summary>
    /// The custom status, or null for none.
    /// </summary>
    [JsonProperty("custom_status")]
    public UserCustomStatusJson? CustomStatus { get; set; }

    // [JsonProperty("activities")]
    // public object[] Activities { get; set; } = Array.Empty<object>();

    /// <summary>
    /// Creates the presence. The status is only set when one is given.
    /// </summary>
    /// <param name="status">The status to send.</param>
    /// <param name="mobile">Whether the session is on a mobile client.</param>
    /// <param name="custom">The custom status, or null for none.</param>
    public PresenceUpdateGatewayData(StatusEnum? status = null, bool? mobile = null, UserCustomStatusJson? custom = null)
    {
        CustomStatus = custom;
        IsMobile = mobile;
        if (status.HasValue)
        {
            Status = status switch
            {
                StatusEnum.Online => "online",
                StatusEnum.Idle => "idle",
                StatusEnum.Dnd => "dnd",
                StatusEnum.Invisible => "invisible",
                _ => Status
            };
        }
    }
}