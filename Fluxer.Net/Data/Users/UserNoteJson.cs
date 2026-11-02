using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A note that a user has written about another user, as the API sends it.
/// </summary>
public class UserNoteJson
{
    /// <summary>
    /// The id of the user who wrote the note.
    /// </summary>
    [JsonProperty("source_user_id")]
    public ulong SourceUserId { get; set; }

    /// <summary>
    /// The id of the user the note is about.
    /// </summary>
    [JsonProperty("target_user_id")]
    public ulong TargetUserId { get; set; }

    /// <summary>
    /// The text of the note.
    /// </summary>
    [JsonProperty("note")]
    public string Note { get; set; }
}
