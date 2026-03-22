using Newtonsoft.Json;

namespace Fluxer.Net;

public class MfaBackupCodeJson
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("consumed")]
    public bool Consumed { get; set; }
}
