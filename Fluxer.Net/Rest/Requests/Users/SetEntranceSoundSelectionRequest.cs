using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Chooses the entrance sound for a scope.
/// </summary>
public class SetEntranceSoundSelectionRequest
{
    /// <summary>
    /// "global", "guilds", "dms" or "guild:{id}".
    /// </summary>
    [JsonProperty("scope_id")]
    public string ScopeId { get; set; }

    /// <summary>
    /// The sound to use, or null to clear the choice.
    /// </summary>
    [JsonProperty("sound_id", NullValueHandling = NullValueHandling.Include)]
    public ulong? SoundId { get; set; }
}
