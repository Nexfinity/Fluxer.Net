namespace Fluxer.Net.Data.Enums;

[Flags]
public enum FriendSourceFlags
{
	None = 0,
	MutualFriends = 1 << 0,
	MutualGuilds = 1 << 1,
	NoRelation = 1 << 2,
}
