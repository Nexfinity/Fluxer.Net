using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A backup code for two-factor authentication.
/// </summary>
public class MfaBackupCodeJson
{
    /// <summary>
    /// The id of the user the code belongs to.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The backup code.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; }

    /// <summary>
    /// Whether the code has already been used.
    /// </summary>
    [JsonProperty("consumed")]
    public bool Consumed { get; set; }
}
