using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Sets the default for sharing voice activity with friends.
/// </summary>
public class VoiceActivitySharingRequest
{
    /// <summary>
    /// The new default, also applied to every existing friend.
    /// </summary>
    [JsonProperty("share_voice_activity")]
    public bool ShareVoiceActivity { get; set; }
}
