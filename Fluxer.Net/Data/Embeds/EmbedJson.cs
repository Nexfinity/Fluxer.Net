using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class EmbedJson : IEmbed
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public string RawType { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <inheritdoc />
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <inheritdoc />
    [JsonProperty("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <inheritdoc />
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <inheritdoc />
    [JsonProperty("author")]
    public EmbedAuthorJson? Author { get; set; }

    /// <inheritdoc />
    [JsonProperty("image")]
    public EmbedMediaJson? Image { get; set; }

    /// <inheritdoc />
    [JsonProperty("thumbnail")]
    public EmbedMediaJson? Thumbnail { get; set; }

    /// <inheritdoc />
    [JsonProperty("footer")]
    public EmbedFooterJson? Footer { get; set; }

    /// <inheritdoc />
    [JsonProperty("fields")]
    public EmbedFieldJson[]? Fields { get; set; }

    /// <inheritdoc />
    [JsonProperty("provider")]
    public EmbedAuthorJson? Provider { get; set; }

    /// <inheritdoc />
    [JsonProperty("video")]
    public EmbedMediaJson? Video { get; set; }

    /// <inheritdoc />
    [JsonProperty("audio")]
    public EmbedMediaJson? Audio { get; set; }

    /// <inheritdoc />
    [JsonProperty("nsfw")]
    public bool IsNsfw { get; set; }

    IEmbedAuthor? IEmbed.Author => Author;

    IEmbedMedia? IEmbed.Thumbnail => Thumbnail;

    IEmbedFooter? IEmbed.Footer => Footer;

    IEmbedField[]? IEmbed.Fields => Fields;

    IEmbedAuthor? IEmbed.Provider => Provider;

    IEmbedMedia? IEmbed.Video => Video;

    IEmbedMedia? IEmbed.Audio => Audio;

    IEmbedMedia? IEmbed.Image => Image;
}

/// <inheritdoc />
public class EmbedFieldJson : IEmbedField
{
    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("value")]
    public string Value { get; set; }

    /// <inheritdoc />
    [JsonProperty("inline")]
    public bool IsInline { get; set; }
}

/// <inheritdoc />
public class EmbedAuthorJson : IEmbedAuthor
{
    /// <inheritdoc />
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }
}

/// <inheritdoc />
public class EmbedFooterJson : IEmbedFooter
{
    /// <inheritdoc />
    [JsonProperty("text")]
    public string? Text { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }
}

/// <inheritdoc />
public class EmbedMediaJson : IEmbedMedia
{
    /// <inheritdoc />
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public ulong Flags { get; set; }

    /// <inheritdoc />
    [JsonProperty("proxy_url")]
    public string? ProxyUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_hash")]
    public string? ContentHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("width")]
    public int? Width { get; set; }

    /// <inheritdoc />
    [JsonProperty("height")]
    public int? Height { get; set; }

    /// <inheritdoc />
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <inheritdoc />
    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }

    /// <inheritdoc />
    [JsonProperty("duration")]
    public int? Duration { get; set; }
}