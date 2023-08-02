namespace Squll.Net.Objects.DataTables;

public enum UserPublicFlags
{
    None = 0,
    Staff = 1 << 0,
    AlphaTester = 1 << 1,
    Verified = 1 << 2,
    Deleted = 1 << 3
}
