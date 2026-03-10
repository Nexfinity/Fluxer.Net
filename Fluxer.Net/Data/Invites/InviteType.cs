namespace Fluxer.Net.Data.Invites;

/// <summary>
/// Represents the type of invite (what the invite is for).
/// </summary>
public enum InviteType
{
	/// <summary>
	/// An invite to join a guild/server.
	/// </summary>
	Guild = 0,

	/// <summary>
	/// An invite to join a group DM.
	/// </summary>
	GroupDm = 1,
}
