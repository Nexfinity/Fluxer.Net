namespace Fluxer.Net.Objects.Data;

[Flags]
public enum SystemChannelFlags
{
	None = 0,
	SuppressJoinNotifications = 1 << 0,
}
