namespace Fluxer.Net.Objects.Data;

[Flags]
public enum GuildMemberFlags
{
	None = 0,
	Deafened = 1 << 0,
	Muted = 1 << 1,
}
