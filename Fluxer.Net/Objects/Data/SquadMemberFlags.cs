namespace Fluxer.Net.Objects.Data;

public enum CommunityMemberFlags
{
    None = 0,
    Deafened = 1 << 0,
    Muted = 1 << 1,
}
