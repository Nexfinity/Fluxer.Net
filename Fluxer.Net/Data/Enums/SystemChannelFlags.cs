namespace Fluxer.Net.Data.Enums;

[Flags]
public enum SystemChannelFlags
{
	None = 0,
	SuppressJoinNotifications = 1 << 0,
}
