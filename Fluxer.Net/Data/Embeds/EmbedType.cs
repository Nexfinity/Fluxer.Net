namespace Fluxer.Net;

/// <summary>
/// The kinds of content an embed can hold.
/// </summary>
public enum EmbedType
{
    /// <summary>
    /// The embed type is not known.
    /// </summary>
    Unknown,

    /// <summary>
    /// A rich embed.
    /// </summary>
    Rich,

    /// <summary>
    /// An article embed.
    /// </summary>
    Article,

    /// <summary>
    /// A link embed.
    /// </summary>
    Link,

    /// <summary>
    /// An image embed.
    /// </summary>
    Image,

    /// <summary>
    /// A video embed.
    /// </summary>
    Video,

    /// <summary>
    /// An audio embed.
    /// </summary>
    Audio,

    /// <summary>
    /// An animated GIF embed.
    /// </summary>
    Gifv,

    /// <summary>
    /// A Bluesky post embed.
    /// </summary>
    BlueSky
}
