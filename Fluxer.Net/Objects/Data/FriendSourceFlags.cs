namespace Fluxer.Net.Objects.Data;

public enum FriendSourceFlags
{
    None = 0,
    Everyone = 1 << 0,
    MutualFriends = 1 << 1,
    MutualGuilds = 1 << 2,
}
