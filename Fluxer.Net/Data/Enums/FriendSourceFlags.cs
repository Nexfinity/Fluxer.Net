namespace Fluxer.Net.Objects.Data;

[Flags]
public enum FriendSourceFlags
{
	None = 0,
	MutualFriends = 1 << 0,
	MutualGuilds = 1 << 1,
	NoRelation = 1 << 2,
}
