using Newtonsoft.Json;

namespace Fluxer.Net;

public class ForumPostDataJson
{
    [JsonProperty("owner")]
    public GuildMemberJson? Owner { get; set; }

    [JsonProperty("first_message")]
    public MessageJson? FirstMessage { get; set; }
}

public class ForumPostDataListJson
{
    [JsonProperty("threads")]
    public Dictionary<ulong, ForumPostDataJson> Threads { get; set; } = new Dictionary<ulong, ForumPostDataJson>();
}
