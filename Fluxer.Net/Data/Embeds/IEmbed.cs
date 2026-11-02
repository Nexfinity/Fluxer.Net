namespace Fluxer.Net;

/// <summary>
/// An embed attached to a message.
/// </summary>
public interface IEmbed
{
    /// <summary>
    /// The type of the embed, as the API sends it.
    /// </summary>
    string RawType { get; }

    /// <summary>
    /// The URL of the embed.
    /// </summary>
    string? Url { get; }

    /// <summary>
    /// The title of the embed.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// The color of the embed sidebar, as a raw 0xRRGGBB value.
    /// </summary>
    int? Color { get; }

    /// <summary>
    /// The ISO 8601 timestamp of the embed content.
    /// </summary>
    DateTimeOffset? Timestamp { get; }

    /// <summary>
    /// The description of the embed.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// The author information of the embed.
    /// </summary>
    IEmbedAuthor? Author { get; }

    /// <summary>
    /// The image of the embed.
    /// </summary>
    IEmbedMedia? Image { get; }

    /// <summary>
    /// The thumbnail of the embed.
    /// </summary>
    IEmbedMedia? Thumbnail { get; }

    /// <summary>
    /// The footer of the embed.
    /// </summary>
    IEmbedFooter? Footer { get; }

    /// <summary>
    /// The fields of the embed.
    /// </summary>
    IEmbedField[]? Fields { get; }

    /// <summary>
    /// The provider of the embed (e.g., YouTube, Twitter).
    /// </summary>
    IEmbedAuthor? Provider { get; }

    /// <summary>
    /// The video of the embed.
    /// </summary>
    IEmbedMedia? Video { get; }

    /// <summary>
    /// The audio of the embed.
    /// </summary>
    IEmbedMedia? Audio { get; }

    /// <summary>
    /// Whether the embed is flagged as NSFW.
    /// </summary>
    bool IsNsfw { get; }
}

/// <summary>
/// A field of an embed, with a name and a value.
/// </summary>
public interface IEmbedField
{
    /// <summary>
    /// The name of the field.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The value of the field.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Whether the field should be displayed inline.
    /// </summary>
    bool IsInline { get; }
}

/// <summary>
/// The author section of an embed.
/// </summary>
public interface IEmbedAuthor
{
    /// <summary>
    /// The name of the author.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// The URL of the author.
    /// </summary>
    string? Url { get; }

    /// <summary>
    /// The URL of the author icon.
    /// </summary>
    string? IconUrl { get; }

    /// <summary>
    /// The proxied URL of the author icon.
    /// </summary>
    string? ProxyIconUrl { get; }
}

/// <summary>
/// The footer section of an embed.
/// </summary>
public interface IEmbedFooter
{
    /// <summary>
    /// The footer text.
    /// </summary>
    string? Text { get; }

    /// <summary>
    /// The URL of the footer icon.
    /// </summary>
    string? IconUrl { get; }

    /// <summary>
    /// The proxied URL of the footer icon.
    /// </summary>
    string? ProxyIconUrl { get; }
}

/// <summary>
/// Media shown in an embed, such as an image or a video.
/// </summary>
public interface IEmbedMedia
{
    /// <summary>
    /// The URL of the media.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// The bitwise flags for this media.
    /// </summary>
    ulong Flags { get; }

    /// <summary>
    /// The proxied URL of the media.
    /// </summary>
    string? ProxyUrl { get; }

    /// <summary>
    /// The MIME type of the media.
    /// </summary>
    string? ContentType { get; }

    /// <summary>
    /// The hash of the media content.
    /// </summary>
    string? ContentHash { get; }

    /// <summary>
    /// The width of the media in pixels.
    /// </summary>
    int? Width { get; }

    /// <summary>
    /// The height of the media in pixels.
    /// </summary>
    int? Height { get; }

    /// <summary>
    /// The description of the media.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// The base64 encoded placeholder image for lazy loading.
    /// </summary>
    string? Placeholder { get; }

    /// <summary>
    /// The duration of the media in seconds.
    /// </summary>
    int? Duration { get; }
}