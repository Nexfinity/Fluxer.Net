namespace Squll.Net.Objects.DataTables;

public enum SpaceFlags
{
    None = 0,
    Sensitive = 1 << 0,
    /// <summary>
    ///     NOTE: this flag will probably be removed.
    /// </summary>
    OwnerOnlyDeleteMe = 1 << 1
}
