using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class ActivityJson : IActivity
{
    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public int Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("timestamps")]
    public ActivityTimestampsJson? Timestamps { get; set; }

    /// <inheritdoc />
    [JsonProperty("details")]
    public string? Details { get; set; }

    /// <inheritdoc />
    [JsonProperty("state")]
    public string? State { get; set; }

    /// <inheritdoc />
    [JsonProperty("emoji")]
    public ActivityEmojiJson? Emoji { get; set; }

    IActivityTimestamps? IActivity.Timestamps => Timestamps;

    IActivityEmoji? IActivity.Emoji => Emoji;
}

/// <inheritdoc />
public class ActivityTimestampsJson : IActivityTimestamps
{
    /// <inheritdoc />
    [JsonProperty("start")]
    public long? Start { get; set; }

    /// <inheritdoc />
    [JsonProperty("end")]
    public long? End { get; set; }
}

/// <inheritdoc />
public class ActivityEmojiJson : IActivityEmoji
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong? Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("animated")]
    public bool Animated { get; set; }
}