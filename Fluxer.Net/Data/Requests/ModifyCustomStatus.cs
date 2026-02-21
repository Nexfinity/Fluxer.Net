using Fluxer.Net.Data.Models;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Requests;

public class ModifyCustomStatus
{
    public ModifyCustomStatus(UserCustomStatus status)
    {
        CustomStatus = status;
    }

    [JsonProperty("custom_status")]
    public UserCustomStatus CustomStatus { get; set; }
}