using Newtonsoft.Json;

namespace Fluxer.Net;

public class PresenceJson
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("activities")]
    public List<ActivityJson>? Activities { get; set; }

    [JsonProperty("client_status")]
    public ClientStatusJson? ClientStatus { get; set; }
}

public class ActivityJson
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    [JsonProperty("timestamps")]
    public ActivityTimestampsJson? Timestamps { get; set; }

    [JsonProperty("details")]
    public string? Details { get; set; }

    [JsonProperty("state")]
    public string? State { get; set; }

    [JsonProperty("emoji")]
    public ActivityEmojiJson? Emoji { get; set; }
}

public class ActivityTimestampsJson
{
    [JsonProperty("start")]
    public long? Start { get; set; }

    [JsonProperty("end")]
    public long? End { get; set; }
}

public class ActivityEmojiJson
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("id")]
    public ulong? Id { get; set; }

    [JsonProperty("animated")]
    public bool Animated { get; set; }
}

public class ClientStatusJson
{
    [JsonProperty("desktop")]
    public string? Desktop { get; set; }

    [JsonProperty("mobile")]
    public string? Mobile { get; set; }

    [JsonProperty("web")]
    public string? Web { get; set; }
}
