namespace Fluxer.Net;

/// <summary>
/// Represents how a message references another message.
/// </summary>
public enum MessageReferenceType
{
	/// <summary>
	/// Standard message reference (typically a reply).
	/// </summary>
	Default = 0,

	/// <summary>
	/// A forwarded message from another channel or conversation.
	/// </summary>
	Forward = 1,
}
