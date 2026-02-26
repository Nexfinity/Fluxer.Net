namespace Fluxer.Net.Data.Enums;

/// <summary>
/// Flags that can be applied to messages. Multiple flags can be combined using bitwise OR.
/// </summary>
[Flags]
public enum MessageFlags
{
	/// <summary>
	/// No flags set.
	/// </summary>
	None = 0,

	/// <summary>
	/// Do not include any embeds when serializing this message (link previews hidden).
	/// </summary>
	SuppressEmbeds = 1 << 2,

	/// <summary>
	/// Message will not trigger push or desktop notifications.
	/// </summary>
	SuppressNotifications = 1 << 12,

	VoiceMessage = 1 << 13,

	CompactAttachments = 1 << 17
}
