namespace Fluxer.Net;

/// <summary>
/// Represents the type of relationship between the current user and another user.
/// </summary>
public enum RelationshipType
{
	/// <summary>
	/// The user is a friend.
	/// </summary>
	Friend = 1,

	/// <summary>
	/// The user is blocked.
	/// </summary>
	Blocked = 2,

	/// <summary>
	/// Incoming friend request from the user.
	/// </summary>
	IncomingRequest = 3,

	/// <summary>
	/// Outgoing friend request sent to the user.
	/// </summary>
	OutgoingRequest = 4,
}
