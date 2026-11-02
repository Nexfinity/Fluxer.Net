using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for setting the current user's custom status.
/// </summary>
public class UpdateCustomStatus
{
    /// <summary>
    /// Creates a request that sets the given custom status.
    /// </summary>
    /// <param name="status">The custom status to set.</param>
    public UpdateCustomStatus(UserCustomStatusJson status)
    {
        CustomStatus = status;
    }

    /// <summary>
    /// The custom status to set.
    /// </summary>
    [JsonProperty("custom_status")]
    public UserCustomStatusJson CustomStatus { get; set; }
}