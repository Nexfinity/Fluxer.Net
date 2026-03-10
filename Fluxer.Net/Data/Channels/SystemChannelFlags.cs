namespace Fluxer.Net;

/// <summary>
/// Flags that control which system messages are sent to the guild's system channel. Multiple flags can be combined using bitwise OR.
/// </summary>
[Flags]
public enum SystemChannelFlags
{
	/// <summary>
	/// No flags set (all system messages enabled).
	/// </summary>
	None = 0,

	/// <summary>
	/// Suppress member join notifications in the system channel.
	/// </summary>
	SuppressJoinNotifications = 1 << 0,
}
