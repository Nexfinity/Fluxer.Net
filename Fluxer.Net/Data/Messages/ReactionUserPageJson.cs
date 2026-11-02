using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A page of the users who reacted to a message with one emoji.
/// </summary>
public class ReactionUserPageJson
{
    /// <summary>
    /// The users on this page.
    /// </summary>
    [JsonProperty("items")]
    public UserJson[] Users { get; set; } = Array.Empty<UserJson>();

    /// <summary>
    /// Whether more users can be fetched.
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    /// <summary>
    /// The id to fetch the next page after, or null on the last page.
    /// </summary>
    [JsonProperty("next_after")]
    public ulong? NextAfterId { get; set; }
}
