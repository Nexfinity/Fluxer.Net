using Fluxer.Net.Data.Users;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

public class ModifyCustomStatus
{
    public ModifyCustomStatus(UserCustomStatus status)
    {
        CustomStatus = status;
    }

    [JsonProperty("custom_status")]
    public UserCustomStatus CustomStatus { get; set; }
}