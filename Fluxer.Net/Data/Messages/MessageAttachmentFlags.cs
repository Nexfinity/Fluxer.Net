namespace Fluxer.Net;

/// <summary>
/// Flags that describe properties of a message attachment. Multiple flags can be combined using bitwise OR.
/// </summary>
[Flags]
public enum MessageAttachmentFlags
{
	/// <summary>
	/// No flags set.
	/// </summary>
	None = 0,

	/// <summary>
	/// Attachment is marked as a spoiler (hidden by default).
	/// </summary>
	IsSpoiler = 1 << 3,

	/// <summary>
	/// Attachment contains explicit/NSFW media.
	/// </summary>
	ContainsExplicitMedia = 1 << 4,

	/// <summary>
	/// Attachment is an animated file (GIF, APNG, etc.).
	/// </summary>
	IsAnimated = 1 << 5,
}
