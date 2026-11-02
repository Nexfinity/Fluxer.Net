using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The owner and first message of a forum post, as the API sends them.
/// </summary>
public class ForumPostDataJson
{
    /// <summary>
    /// The member who created the post.
    /// </summary>
    [JsonProperty("owner")]
    public GuildMemberJson? Owner { get; set; }

    /// <summary>
    /// The first message of the post.
    /// </summary>
    [JsonProperty("first_message")]
    public MessageJson? FirstMessage { get; set; }
}

/// <summary>
/// The post data of several forum posts, as the API sends them.
/// </summary>
public class ForumPostDataListJson
{
    /// <summary>
    /// The post data, by thread ID.
    /// </summary>
    [JsonProperty("threads")]
    public Dictionary<ulong, ForumPostDataJson> Threads { get; set; } = new Dictionary<ulong, ForumPostDataJson>();
}
