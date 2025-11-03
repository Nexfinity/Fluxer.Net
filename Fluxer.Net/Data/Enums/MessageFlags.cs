namespace Fluxer.Net.Data.Enums;

[Flags]
public enum MessageFlags
{
	None = 0,
	SuppressEmbeds = 1 << 2,
	SuppressNotifications = 1 << 12,
}
