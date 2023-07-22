namespace Squll.Net.Objects;

public enum SquadMemberFlags
{
    None = 0,
    Deafened = 1 << 0,
    Muted = 1 << 1,
}
