namespace Fluxer.Net.Objects.Data;

[Flags]
public enum ChannelFlags
{
	None = 0,
	Sensitive = 1 << 0,
	OwnerOnlyDeleteMe = 1 << 1,
	ResourceChannel = 1 << 2,
	ChannelDeleteLocked = 1 << 3,
}
