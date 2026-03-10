namespace Fluxer.Net.Data.Users;

/// <summary>
/// Represents the explicit content filter level for a user's direct messages.
/// </summary>
public enum UserExplicitContentFilterType
{
	/// <summary>
	/// Explicit content filtering is disabled (no scanning).
	/// </summary>
	Disabled = 0,

	/// <summary>
	/// Scan messages from users who are not friends.
	/// </summary>
	NonFriends = 1,

	/// <summary>
	/// Scan messages from all users (friends and non-friends).
	/// </summary>
	FriendsAndNonFriends = 2,
}
