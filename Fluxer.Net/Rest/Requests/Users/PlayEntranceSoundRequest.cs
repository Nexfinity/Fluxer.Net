using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Plays an entrance sound for everyone in a voice channel.
/// </summary>
public class PlayEntranceSoundRequest
{
    /// <summary>
    /// The sound from your library to play.
    /// </summary>
    [JsonProperty("sound_id")]
    public ulong SoundId { get; set; }
}
