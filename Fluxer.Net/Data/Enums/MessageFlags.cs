namespace Fluxer.Net.Objects.Data;

[Flags]
public enum MessageFlags
{
	None = 0,
	SuppressEmbeds = 1 << 2,
	SuppressNotifications = 1 << 12,
}
