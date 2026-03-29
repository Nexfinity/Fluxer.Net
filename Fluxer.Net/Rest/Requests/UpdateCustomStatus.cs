using Newtonsoft.Json;

namespace Fluxer.Net;

public class UpdateCustomStatus
{
    public UpdateCustomStatus(UserCustomStatusJson status)
    {
        CustomStatus = status;
    }

    [JsonProperty("custom_status")]
    public UserCustomStatusJson CustomStatus { get; set; }
}