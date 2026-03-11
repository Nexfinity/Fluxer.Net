using Newtonsoft.Json;

namespace Fluxer.Net;

public class ModifyCustomStatus
{
    public ModifyCustomStatus(UserCustomStatus status)
    {
        CustomStatus = status;
    }

    [JsonProperty("custom_status")]
    public UserCustomStatus CustomStatus { get; set; }
}