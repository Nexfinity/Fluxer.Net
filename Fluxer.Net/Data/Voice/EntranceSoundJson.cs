using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A sound in the entrance sound library.
/// </summary>
public class EntranceSoundJson
{
    /// <summary>
    /// The sound id.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The display label.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The content hash of the audio.
    /// </summary>
    [JsonProperty("hash")]
    public string Hash { get; set; }

    /// <summary>
    /// The audio file extension.
    /// </summary>
    [JsonProperty("extension")]
    public string Extension { get; set; }

    /// <summary>
    /// The MIME type of the audio.
    /// </summary>
    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    /// <summary>
    /// The length of the sound in milliseconds.
    /// </summary>
    [JsonProperty("duration_ms")]
    public int DurationMs { get; set; }

    /// <summary>
    /// The size of the audio in bytes.
    /// </summary>
    [JsonProperty("size_bytes")]
    public int SizeBytes { get; set; }

    /// <summary>
    /// The URL the audio is served from.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// When the sound was uploaded.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// The sound chosen for a scope.
/// </summary>
public class EntranceSoundSelectionJson
{
    /// <summary>
    /// "global", "guilds", "dms" or "guild:{id}".
    /// </summary>
    [JsonProperty("scope_id")]
    public string ScopeId { get; set; }

    /// <summary>
    /// The chosen sound id.
    /// </summary>
    [JsonProperty("sound_id")]
    public ulong SoundId { get; set; }
}

/// <summary>
/// The entrance sound library with the active selection for each scope.
/// </summary>
public class EntranceSoundLibraryJson
{
    /// <summary>
    /// The saved sounds.
    /// </summary>
    [JsonProperty("sounds")]
    public EntranceSoundJson[] Sounds { get; set; }

    /// <summary>
    /// The active sound for each scope.
    /// </summary>
    [JsonProperty("selections")]
    public EntranceSoundSelectionJson[] Selections { get; set; }
}
