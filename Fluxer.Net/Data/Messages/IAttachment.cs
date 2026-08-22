namespace Fluxer.Net;

public interface IAttachment : ISnowflake
{
    /// <summary>
    /// The name of the attached file.
    /// </summary>
    string Filename { get; }

    /// <summary>
    /// The size of the attachment in bytes.
    /// </summary>
    ulong Size { get; }

    /// <summary>
    /// Attachment flags.
    /// </summary>
    AttachmentFlag Flags { get; }

    /// <summary>
    /// The title of the attachment.
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// The description of the attachment.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// The MIME type of the attachment.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// The hash of the attachment content.
    /// </summary>
    string? ContentHash { get; }

    /// <summary>
    /// The URL of the attachment.
    /// </summary>
    string? Url { get; }

    /// <summary>
    /// The proxied URL of the attachment.
    /// </summary>
    string? ProxyUrl { get; }

    /// <summary>
    /// The width of the attachment in pixels (for images/videos).
    /// </summary>
    int? Width { get; }

    /// <summary>
    /// The height of the attachment in pixels (for images/videos).
    /// </summary>
    int? Height { get; }

    /// <summary>
    /// The base64 encoded placeholder image for lazy loading.
    /// </summary>
    string? Placeholder { get; }

    /// <summary>
    /// Whether the attachment is flagged as NSFW.
    /// </summary>
    bool? IsNsfw { get; }

    /// <summary>
    /// The duration of the media in seconds.
    /// </summary>
    ulong? Duration { get; }

    /// <summary>
    /// The base64 encoded audio waveform data.
    /// </summary>
    string? Waveform { get; }

    /// <summary>
    /// The ISO 8601 timestamp when the attachment URL expires.
    /// </summary>
    DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    /// Whether the attachment URL has expired.
    /// </summary>
    bool? IsExpired { get; }
}
