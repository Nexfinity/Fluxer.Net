using Newtonsoft.Json;

namespace Fluxer.Net;

public class ModifyCustomStatus
{
    public ModifyCustomStatus(UserCustomStatusJson status)
    {
        CustomStatus = status;
    }

    [JsonProperty("custom_status")]
    public UserCustomStatusJson CustomStatus { get; set; }
}