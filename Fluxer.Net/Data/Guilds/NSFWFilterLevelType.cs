namespace Fluxer.Net;

/// <summary>
/// Represents the NSFW (Not Safe For Work) filter level for a user.
/// </summary>
public enum NSFWFilterLevelType
{
	/// <summary>
	/// NSFW filtering is disabled.
	/// </summary>
	Disabled = 0,

	/// <summary>
	/// Filter NSFW content from users who are not friends.
	/// </summary>
	NonFriends = 1,

	/// <summary>
	/// Filter NSFW content from all users (friends and non-friends).
	/// </summary>
	FriendsAndNonFriends = 2,
}
