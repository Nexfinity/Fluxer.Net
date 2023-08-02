namespace Squll.Net.Objects.DataTables;

public enum FriendSourceFlags
{
    None = 0,
    Everyone = 1 << 0,
    MutualFriends = 1 << 1,
    MutualSquads = 1 << 2,
}
