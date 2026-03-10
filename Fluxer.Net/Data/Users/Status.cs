namespace Fluxer.Net;

/// <summary>
/// Represents a user's online status/presence.
/// </summary>
public enum Status
{
	/// <summary>
	/// User is online and active.
	/// </summary>
	Online,

	/// <summary>
	/// User is in "Do Not Disturb" mode (busy, notifications suppressed).
	/// </summary>
	Dnd,

	/// <summary>
	/// User is idle/away from keyboard.
	/// </summary>
	Idle,

	/// <summary>
	/// User appears offline to others (invisible mode).
	/// </summary>
	Invisible,
}
