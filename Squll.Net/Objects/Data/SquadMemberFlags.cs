namespace Squll.Net.Objects.Data;

public enum SquadMemberFlags
{
    None = 0,
    Deafened = 1 << 0,
    Muted = 1 << 1,
}
